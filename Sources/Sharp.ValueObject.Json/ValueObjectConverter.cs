using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class ValueObjectConverter<TValue, TValueObject> : JsonConverter<TValueObject>
        where TValue : IEquatable<TValue>
        where TValueObject : SingleValueObject<TValue, TValueObject>
    {
        public ValueObjectConverter() : this(EqualityComparer<TValue>.Default) { }

        public ValueObjectConverter(IEqualityComparer<TValue> equalityComparer)
        {
            EqualityComparer = equalityComparer;
        }

        public IEqualityComparer<TValue> EqualityComparer { get; }

        public override TValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TValue? value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            return value is not null
                ? SingleValueObject<TValue, TValueObject>.Create(value, EqualityComparer)
                : null;
        }

        public override void Write(Utf8JsonWriter writer, TValueObject valueObject, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, valueObject.Value, options);
        }
    }
}
