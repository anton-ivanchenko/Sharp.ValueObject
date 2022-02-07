using System.Text.Json;

namespace Sharp.ValueObject.Json
{
    public class StringValueObjectConverter<TValueObject> : ValueObjectConverter<string, TValueObject>
        where TValueObject : StringValueObject<TValueObject>
    {
        public StringValueObjectConverter() : base(StringComparer.OrdinalIgnoreCase) { }

        protected override string? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            throw new InvalidOperationException($@"Unexpected json token ""{reader.TokenType}""");
        }

        public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
