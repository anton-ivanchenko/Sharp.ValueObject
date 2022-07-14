using Sharp.ValueObject.Json.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class EnumerableValueObjectConverter<TValue, TValueObject, TCollection> : JsonConverter<TCollection>
        where TValue : notnull
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
            using var document = JsonDocument.ParseValue(ref reader);
            var array = document.RootElement;

            var collectionItems = new List<TValueObject?>(capacity: array.GetArrayLength());

            foreach (var item in array.EnumerateArray())
            {
                string rawJsonValue = item.GetRawText();
                var valueObject = JsonSerializer.Deserialize<TValueObject>(rawJsonValue, options);
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
