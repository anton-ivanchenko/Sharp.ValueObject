using Sharp.ValueObject.Json.Internal;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class ValueObjectConverterFactory : JsonConverterFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<JsonConverter>> _cachedFactories;

        static ValueObjectConverterFactory()
        {
            _cachedFactories = new ConcurrentDictionary<Type, Func<JsonConverter>>();
        }

        public override bool CanConvert(Type typeToConvert)
            => ValueObject.IsSingleValueObjectType(typeToConvert) || ValueObject.IsSingleValueObjectCollectionType(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var factoryMethod = _cachedFactories.GetOrAdd(typeToConvert, t => ReflectionHelper.GenerateJsonConverterFactory(t));
            var jsonConverter = factoryMethod.Invoke();

            return jsonConverter;
        }
    }
}
