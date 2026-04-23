using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;




namespace ACS.Swagger
{
    /// <summary>
    /// Enum Schema Filter
    /// </summary>
    public class EnumSchemaFilter : IDocumentFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var property in swaggerDoc.Components.Schemas.Where(x => x.Value?.Enum?.Count > 0))
            {
                IList<IOpenApiAny> propertyEnums = property.Value.Enum;
                if (propertyEnums != null && propertyEnums.Count > 0)
                {
                    property.Value.Description += DescribeEnum(propertyEnums, property.Key);
                }
            }

            // add enum descriptions to input parameters
            foreach (var pathItem in swaggerDoc.Paths)
            {
                DescribeEnumParameters(pathItem.Value.Operations, swaggerDoc, context.ApiDescriptions, pathItem.Key);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="swaggerDoc"></param>
        /// <param name="apiDescriptions"></param>
        /// <param name="path"></param>
        private void DescribeEnumParameters(IDictionary<OperationType, OpenApiOperation> operations, OpenApiDocument swaggerDoc, IEnumerable<ApiDescription> apiDescriptions, string path)
        {
            path = path.Trim('/');
            if (operations != null)
            {
                var pathDescriptions = apiDescriptions.Where(a => a.RelativePath == path);
                foreach (var oper in operations)
                {
#pragma warning disable CS8602 // Converting null literal or possible null value to non-nullable type.
                    var operationDescription = pathDescriptions.FirstOrDefault(a => a.HttpMethod.Equals(oper.Key.ToString(), StringComparison.InvariantCultureIgnoreCase));

                    foreach (var param in oper.Value.Parameters)
                    {
                        if (operationDescription == null) continue;
                        var parameterDescription = operationDescription.ParameterDescriptions.FirstOrDefault(a => a.Name == param.Name);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        if (parameterDescription != null && TryGetEnumType(parameterDescription.Type, out Type enumType))
                        {
                            var paramEnum = swaggerDoc.Components.Schemas.FirstOrDefault(x => x.Key == enumType.Name);
                            if (paramEnum.Value != null)
                            {
                                param.Description += DescribeEnum(paramEnum.Value.Enum, paramEnum.Key);
                            }
                        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    }
#pragma warning restore CS8602 // Converting null literal or possible null value to non-nullable type.
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private bool TryGetEnumType(Type type, out Type? enumType)
        {

            if (type.IsEnum)
            {
                enumType = type;
                return true;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null && underlyingType.IsEnum == true)
                {
                    enumType = underlyingType;
                    return true;
                }
            }
            else
            {
                Type? underlyingType = GetTypeIEnumerableType(type);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    enumType = underlyingType;
                    return true;
                }
                else
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        underlyingType = GetTypeIEnumerableType(interfaceType);
                        if (underlyingType != null && underlyingType.IsEnum)
                        {
                            enumType = underlyingType;
                            return true;
                        }
                    }
                }
            }

            enumType = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type? GetTypeIEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var underlyingType = type.GetGenericArguments()[0];
                if (underlyingType.IsEnum)
                {
                    return underlyingType;
                }
            }

            return null;
        }

        /// <summary>
        /// Get Enum Type ByName
        /// </summary>
        /// <param name="enumTypeName"></param>
        /// <returns></returns>
        private static Type? GetEnumTypeByName(string enumTypeName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == enumTypeName);
        }

        /// <summary>
        /// Describe Enum
        /// </summary>
        /// <param name="enums"></param>
        /// <param name="proprtyTypeName"></param>
        /// <returns></returns>
        private static string? DescribeEnum(IList<IOpenApiAny> enums, string proprtyTypeName)
        {
            List<string> enumDescriptions = [];
            var enumType = GetEnumTypeByName(proprtyTypeName);
            if (enumType == null) return null;
            enumDescriptions.AddRange(from OpenApiInteger enumOption in enums
                                      let enumInt = enumOption.Value
                                      select string.Format("{0} = {1}", enumInt, Enum.GetName(enumType, enumInt)));
            return string.Join(", ", [.. enumDescriptions]);
        }

    }
}
