using System.Collections.Concurrent;
using System.Linq.Expressions;
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
        {
            return typeof(ValueObject).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return _cachedFactories.GetOrAdd(typeToConvert, t => CreateFactory(t)).Invoke();
        }

        private static Func<JsonConverter> CreateFactory(Type typeToConvert)
        {
            for (Type type = typeToConvert; type != typeof(object); type = type.BaseType!)
            {
                if (!type.IsGenericType)
                    continue;

                var typeDefinition = type.GetGenericTypeDefinition();

                if (typeDefinition == typeof(StringValueObject<>))
                {
                    var converterType = typeof(StringValueObjectConverter<>).MakeGenericType(typeToConvert);
                    var method = Expression.Lambda<Func<JsonConverter>>(Expression.New(converterType));
                    return method.Compile();
                }
                else if (typeDefinition == typeof(ValueObject<,>))
                {
                    var converterType = typeof(GenericValueObjectConverter<,>).MakeGenericType(type.GetGenericArguments());
                    var method = Expression.Lambda<Func<JsonConverter>>(Expression.New(converterType));
                    return method.Compile();
                }
            }

            throw new InvalidOperationException($@"The type ""{typeToConvert}"" cannot be handled");
        }
    }
}
