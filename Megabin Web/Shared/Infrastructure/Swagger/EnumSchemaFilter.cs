using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Megabin_Web.Shared.Infrastructure.Swagger
{
    /// <summary>
    /// Schema filter that configures enums to be represented as strings in the OpenAPI spec.
    /// This ensures generated client code uses string enums instead of numeric values.
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

                // Add enum values as strings
                foreach (var enumValue in Enum.GetValues(context.Type))
                {
                    schema.Enum.Add(new OpenApiString(enumValue.ToString()));
                }
            }
        }
    }
}
