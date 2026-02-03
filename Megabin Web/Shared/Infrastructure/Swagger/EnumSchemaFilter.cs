using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Megabin_Web.Shared.Infrastructure.Swagger
{
    /// <summary>
    /// Schema filter that configures enums to be represented as strings in the OpenAPI spec.
    /// This ensures generated client code uses string enums instead of numeric values.
    /// Uses camelCase naming to match runtime JSON serialization.
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Type = "string";
                schema.Format = null;
                schema.Enum.Clear();

                // Add enum values as camelCase strings to match JsonStringEnumConverter configuration
                foreach (var enumValue in Enum.GetValues(context.Type))
                {
                    var enumName = enumValue.ToString()!;
                    var camelCaseName = JsonNamingPolicy.CamelCase.ConvertName(enumName);
                    schema.Enum.Add(new OpenApiString(camelCaseName));
                }
            }
        }
    }
}
