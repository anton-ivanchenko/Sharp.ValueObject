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

            if (schema.Properties.Count == 1)
            {
                var valueProperty = schema.Properties.Values.First();

                schema.Type = valueProperty.Type;
                valueProperty.Properties.Clear();
            }
        }
    }
}
