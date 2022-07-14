using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class ValueObjectConverter<TValue, TValueObject> : JsonConverter<TValueObject>
        where TValue : notnull
        where TValueObject : SingleValueObject<TValue, TValueObject>
    {
        public override TValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TValue? value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            if (value is null)
                return null;

            return SingleValueObject<TValue, TValueObject>.Create(value);
        }

        public override void Write(Utf8JsonWriter writer, TValueObject valueObject, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, valueObject.Value, options);
        }
    }
}
