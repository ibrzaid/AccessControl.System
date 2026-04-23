
using ACS.Background;
using ACS.Cache.Service.V1.Interfaces;
using ACS.Cache.Service.V1.Services;
using ACS.Handler;
using ACS.HealthChecks;
using ACS.Helper;
using ACS.Helper.V1;
using ACS.Middleware;
using ACS.Service.V1.Interfaces;
using ACS.Service.V1.Services;
using ACS.Swagger;
using ACS.Webhook.WebService.Middleware;
using ACS.Webhook.WebService.Notification;
using ACS.Webhook.WebService.Services.V1.Interfaces;
using ACS.Webhook.WebService.Services.V1.Services;
using Asp.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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


//// Add services to the container.
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

// Add services to the container.

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
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Setup Integration", Version = "v1", Description = "Setup Integration", });
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


    // Add Basic Authentication to Swagger
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication with username and password"
    });


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
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

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;

                if (path.StartsWithSegments("/graphql") || path.StartsWithSegments("/ui/graphiql") || path.StartsWithSegments("/ui/altair") || path.StartsWithSegments("/ui/voyager"))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }

                if (path.StartsWithSegments("/webhook"))
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
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

builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<FindClaimHelper>();
builder.Services.AddSingleton<TokenManagerMiddleware>();
builder.Services.AddSingleton<DynamicKeyJwtValidationHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<ILicenseManager, LicenseManager>();
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IDashboardService, DashboardService>();


builder.Services.AddHostedService<BackgroundTaskService>();

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

var serviceProvider = builder.Services.BuildServiceProvider();
var licenseManager = serviceProvider.GetRequiredService<ILicenseManager>();
var license = licenseManager.GetLicense();
var redisConnection = license?.Cache?.Connection ?? "";

builder.Services.AddHealthChecks()
    .AddCheck<LicenseHealthCheck>("license",
    failureStatus: HealthStatus.Unhealthy,
        tags: ["license", "critical"])
    .AddCheck<RedisHealthCheck>("redis",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["redis", "critical"]);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

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

using (var scope = app.Services.CreateScope())
{
    var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
    dashboardService.StartNotify();
}
app.MapHub<NotificationHub>("/webhook");
app.MapControllers();



app.Run();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'