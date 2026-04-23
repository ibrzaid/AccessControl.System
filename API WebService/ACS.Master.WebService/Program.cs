using ACS.Handler;
using ACS.HealthChecks;
using ACS.Helper;
using ACS.Helper.V1;
using ACS.Master.WebService.GraphQL;
using ACS.Master.WebService.GraphQL.V1.DataLoader;
using ACS.Master.WebService.Middleware;
using ACS.Master.WebService.Services.V1.Interfaces;
using ACS.Master.WebService.Services.V1.Services;
using ACS.Middleware;
using ACS.Service;
using ACS.Service.V1.Interfaces;
using ACS.Service.V1.Services;
using ACS.Swagger;
using Asp.Versioning;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Types;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;


#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

var builder = WebApplication.CreateBuilder(args);

const string AllowAllHeadersPolicy = "AllowAll";

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();


builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAllHeadersPolicy, policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true)); // Allow all origins
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});


// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new HeaderApiVersionReader("Accept-Version");
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Access Control Solution", Version = "v1", Description = "Access Control Solution", });
    options.OperationFilter<SwaggerParameterFilters>();
    options.DocumentFilter<SwaggerVersionMapping>();
    options.DocumentFilter<EnumSchemaFilter>();

    var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    if (!string.IsNullOrWhiteSpace(location))
    {
        var files = Directory.GetFiles(Path.Combine(location), "*.xml");
        if (files != null)
        {
            foreach (var file in files)
            {
                options.IncludeXmlComments(file, true);
            }
        }
    }

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                    }
                },
                Array.Empty<string>()
        }
    });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.Audience = TokenHelper.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = TokenHelper.Issuer,
            ValidAudience = TokenHelper.Audience,
            ClockSkew = TimeSpan.Zero
        };
        options.TokenHandlers.Clear();

        var serviceProvider = builder.Services?.BuildServiceProvider()!;
        var handler = serviceProvider?.GetRequiredService<DynamicKeyJwtValidationHandler>();
        options.TokenHandlers.Add(handler!);

        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                var hasAllowAnonymous = context.HttpContext.GetEndpoint()?.Metadata
                                .GetMetadata<AllowAnonymousAttribute>() != null;
                if (hasAllowAnonymous) context.NoResult();
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;

                if (path.StartsWithSegments("/graphql") || path.StartsWithSegments("/ui/graphiql") || path.StartsWithSegments("/ui/altair") || path.StartsWithSegments("/ui/voyager"))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }

                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/Notification/v1")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = async context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    var payload = new
                    {
                        Success = false,
                        Message = "Token expired",
                        ErrorCode = "S02"
                    };

                    string jsonString = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

                    context.NoResult();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(jsonString);
                    await context.Response.CompleteAsync();
                }
                else
                {
                    var payload = new
                    {
                        Success = false,
                        Message = context.Exception.ToString(),
                        ErrorCode = "S02"
                    };


                    string jsonString = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

                    context.NoResult();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(jsonString);
                    await context.Response.CompleteAsync();

                }
            }
        };
    });

// MemoryCache
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<FindClaimHelper>();
builder.Services.AddSingleton<TokenManagerMiddleware>();
builder.Services.AddSingleton<DynamicKeyJwtValidationHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<ILicenseManager, LicenseManager>();
builder.Services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
builder.Services.AddSingleton<ICountryService, CountryService>();
builder.Services.AddSingleton<IPlateCategoryService, PlateCategoryService>();
builder.Services.AddSingleton<IPlateStateService, PlateStateService>();
builder.Services.AddSingleton<IPlateCategoryDetailService, PlateCategoryDetailService>();
builder.Services.AddSingleton<ISetupReferenceService, SetupReferenceService>();

builder.Services.AddSingleton<CountryDataLoader>();
builder.Services.AddSingleton<PlateStateDataLoader>();
builder.Services.AddSingleton<PlateCategoryDataLoader>();
builder.Services.AddSingleton<PlateCategoryDetailDataLoader>();
builder.Services.AddSingleton<CountryPlateStatesDataLoader>();
builder.Services.AddSingleton<CountryPlateCategoryDetailsDataLoader>();
builder.Services.AddSingleton<PlateStateCategoryDetailsDataLoader>();
builder.Services.AddSingleton<PlateCategoryDetailsDataLoader>();
builder.Services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
builder.Services.AddScoped<DataLoaderContext>();

var graphQLTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => !t.IsAbstract &&
                (typeof(IGraphType).IsAssignableFrom(t) ||
                 typeof(ObjectGraphType).IsAssignableFrom(t)));
foreach (var type in graphQLTypes)
    builder.Services.AddScoped(type);

// Register GraphQL components
builder.Services.AddScoped<RootQuery>();
builder.Services.AddScoped<ISchema, AppSchema>();

builder.Services.AddGraphQL(option =>
{
    var isDevelopment = builder.Environment.IsDevelopment();

    option.AddSystemTextJson();
    option.AddErrorInfoProvider(opt =>
    {
        opt.ExposeExceptionDetails = isDevelopment;
        opt.ExposeExtensions = isDevelopment;
        opt.ExposeCodes = isDevelopment;
    });
    option.AddSchema<AppSchema>();
    option.AddGraphTypes(typeof(RootQuery).Assembly);
    option.AddDataLoader();
});




builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);



builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "ar" }; // Specify supported languages
    options.DefaultRequestCulture = new RequestCulture("en"); // Default to English
    options.SupportedCultures = [.. supportedCultures.Select(c => new CultureInfo(c))];
    options.SupportedUICultures = options.SupportedCultures;
});

builder.Services.AddHealthChecks()
    .AddCheck<LicenseHealthCheck>("license",
    failureStatus: HealthStatus.Unhealthy,
        tags: ["license", "critical"]);

builder.Services.AddHttpClient();

var app = builder.Build();


app.UseCors(AllowAllHeadersPolicy);
app.UseRouting();

{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var actionDescriptorCollectionProvider = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
        foreach (var version in actionDescriptorCollectionProvider.ActionDescriptors.Items
                                .OfType<ControllerActionDescriptor>()
                                .Where(ad => ad.EndpointMetadata
                                             .OfType<ApiExplorerSettingsAttribute>()
                                             .Any(attr => !attr.IgnoreApi))
                                .SelectMany(ad => ad.EndpointMetadata
                                                    .OfType<ApiVersionAttribute>()
                                                    .Select(attr => attr.Versions.FirstOrDefault()))
                                .Distinct()
                                .ToList())
        {
            if (version == null) continue;
            options.SwaggerEndpoint($"/swagger/v{version.MajorVersion}/swagger.json", $"V{version.MajorVersion}");
        }
    });
    app.UseGraphQLGraphiQL("/ui/graphiql");
    app.UseGraphQLAltair("/ui/altair");
    app.UseGraphQLVoyager("/ui/voyager");

    IdentityModelEventSource.ShowPII = true;
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/license", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate = reg => reg.Tags.Contains("license"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.UseRequestLocalization();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<TokenManagerMiddleware>();
app.UseMiddleware<SerilogMiddleware>();
app.UseMiddleware<LicenseMiddleware>();


app.MapControllers();
app.UseGraphQL<ISchema>("/graphql");

app.Run();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'