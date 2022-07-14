using Sharp.ValueObject.SingleValueObjects;
using System;
using System.Text.Json;

namespace Sharp.ValueObject.Json
{
    public class StringValueObjectConverter<TValueObject> : ValueObjectConverter<string, TValueObject>
        where TValueObject : SingleValueObject<string, TValueObject>
    {
        public override TValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (value is null)
                return null;

            return StringValueObject<TValueObject>.ParseOrCreate(value);
        }

        public override void Write(Utf8JsonWriter writer, TValueObject valueObject, JsonSerializerOptions options)
        {
            writer.WriteStringValue(valueObject.Value);
        }
    }
}
