using Sharp.ValueObject.Json.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class EnumerableValueObjectConverter<TValue, TValueObject, TCollection> : JsonConverter<TCollection>
        where TValue : IEquatable<TValue>
        where TValueObject : SingleValueObject<TValue, TValueObject>
        where TCollection : IEnumerable<TValueObject>
    {
        private static readonly Func<IEnumerable<TValueObject?>, TCollection> _collectionFactory;

        static EnumerableValueObjectConverter()
        {
            _collectionFactory = ReflectionHelper.GenerateCollectionFactory<TValueObject, TCollection>();
        }

        public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var array = JsonElement.ParseValue(ref reader);

            var collectionItems = new List<TValueObject?>(capacity: array.GetArrayLength());

            foreach (var item in array.EnumerateArray())
            {
                string rawJsonValue = item.GetRawText();
                TValueObject? valueObject = JsonSerializer.Deserialize<TValueObject>(rawJsonValue, options);
                collectionItems.Add(valueObject);
            }

            return _collectionFactory.Invoke(collectionItems);
        }

        public override void Write(Utf8JsonWriter writer, TCollection collection, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (TValueObject? item in collection)
            {
                JsonSerializer.Serialize(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }
}
