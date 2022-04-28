using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Sharp.ValueObject.Swashbuckle
{
    public class ValueObjectSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!ValueObject.IsSingleValueObjectType(context.Type))
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
