
using Asp.Versioning;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace ACS.Swagger
{
    /// <summary>
    /// Swagger Parameter Filters
    /// </summary>
    public class SwaggerParameterFilters : IOperationFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            try
            {
                var versionParameter = operation.Parameters.Single(p => p.Name == "Accept-Version");

                var maps = context.MethodInfo.GetCustomAttributes(true).OfType<MapToApiVersionAttribute>().SelectMany(attr => attr.Versions).ToList();
                var version = maps[0].MajorVersion;
                if (versionParameter != null)
                {
                    versionParameter.Schema = new OpenApiSchema { Default = new OpenApiString(version.ToString()) };
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}