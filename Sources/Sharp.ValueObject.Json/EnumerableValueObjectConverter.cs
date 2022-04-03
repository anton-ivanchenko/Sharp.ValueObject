using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class EnumerableValueObjectConverter<TValue, TValueObject, TCollection> : JsonConverter<TCollection>
        where TValue : IEquatable<TValue>
        where TValueObject : SingleValueObject<TValue, TValueObject>
        where TCollection : IEnumerable<TValueObject>
    {
        private static readonly Func<IEnumerable<TValueObject>, TCollection> _collectionFactory;

        static EnumerableValueObjectConverter()
        {
            _collectionFactory = CreateCollectionFactory();
        }

        public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var array = JsonElement.ParseValue(ref reader);

            int collectionItemsCapacity = array.GetArrayLength();
            var collectionItems = new List<TValueObject>(collectionItemsCapacity);

            foreach (var item in array.EnumerateArray())
            {
                string rawJsonValue = item.GetRawText();
                TValueObject? valueObject = JsonSerializer.Deserialize<TValueObject>(rawJsonValue, options);

                if (valueObject == null)
                {
                    throw new InvalidOperationException(@$"The value ""{rawJsonValue}"" cannot be used to create value of ""{typeof(TValueObject)}"" type");
                }

                collectionItems.Add(valueObject);
            }

            Debug.Assert(collectionItems.Capacity == collectionItemsCapacity);
            return _collectionFactory.Invoke(collectionItems);
        }

        public override void Write(Utf8JsonWriter writer, TCollection collection, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (TValueObject item in collection)
            {
                JsonSerializer.Serialize(writer, item, options);
            }

            writer.WriteEndArray();
        }

        private static Func<IEnumerable<TValueObject>, TCollection> CreateCollectionFactory()
        {
            return typeof(TCollection).IsArray
                ? CreateEnumerableArrayFactory()
                : CreateEnumerableCollectionFactory();

            static Func<IEnumerable<TValueObject>, TCollection> CreateEnumerableArrayFactory()
            {
                MethodInfo toArrayMethod = typeof(Enumerable)
                    .GetMethod(nameof(Enumerable.ToArray))
                    !.MakeGenericMethod(typeof(TValueObject));

                var parameter = Expression.Parameter(typeof(IEnumerable<TValueObject>));

                var expression = Expression.Lambda<Func<IEnumerable<TValueObject>, TCollection>>(
                    Expression.Call(toArrayMethod, parameter), parameter);

                return expression.Compile();
            }

            static Func<IEnumerable<TValueObject>, TCollection> CreateEnumerableCollectionFactory()
            {
                ConstructorInfo? collectionConstructor = typeof(TCollection)
                    .GetConstructor(new[] { typeof(IEnumerable<TValueObject>) });

                if (collectionConstructor == null)
                {
                    throw new InvalidOperationException($@"The collection ""{typeof(TCollection)}"" does not have constructor with IEnumerable parameter");
                }

                var parameter = Expression.Parameter(typeof(IEnumerable<TValueObject>));

                var expression = Expression.Lambda<Func<IEnumerable<TValueObject>, TCollection>>(
                    Expression.New(collectionConstructor, parameter), parameter);

                return expression.Compile();
            }
        }
    }
}
