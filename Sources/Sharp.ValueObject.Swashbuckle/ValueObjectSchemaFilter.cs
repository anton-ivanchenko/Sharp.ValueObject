using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sharp.ValueObject.Swashbuckle
{
    public class ValueObjectSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!ValueObject.IsValueObjectType(context.Type))
                return;

            var valueProperties = schema.Properties
                .Where(c => string.Equals(c.Key, "value", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToList();

            if (valueProperties.Count == 1)
            {
                var valueProperty = valueProperties[0];

                schema.Type = valueProperty.Type;
                valueProperty.Properties.Clear();
            }
        }
    }
}
