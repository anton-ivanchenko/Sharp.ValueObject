using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
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
            => ValueObject.IsValueObjectType(typeToConvert);

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return _cachedFactories.GetOrAdd(typeToConvert, t => CreateFactory(t)).Invoke();
        }

        private static Func<JsonConverter> CreateFactory(Type typeToConvert)
        {
            Type genericValueObjectType = ValueObject.GetGenericValueObjectType(typeToConvert);
            Type innerValueType = ValueObject.GetInnerValueType(genericValueObjectType);

            Type[] genericArguments = genericValueObjectType.GetGenericArguments();
            Type converterType = typeof(ValueObjectConverter<,>).MakeGenericType(genericArguments);

            ConstructorInfo? converterConstructor = converterType
                .GetConstructor(new[] { typeof(IEqualityComparer<>).MakeGenericType(innerValueType) });

            Debug.Assert(converterConstructor != null);

            var factoryMethod = Expression.Lambda<Func<JsonConverter>>(
                Expression.New(converterConstructor, GetEqualityComparer(innerValueType)));

            return factoryMethod.Compile();
        }

        private static Expression GetEqualityComparer(Type type)
        {
            if (type == typeof(string))
            {
                PropertyInfo? stringComparerProperty = typeof(StringComparer)
                    .GetProperty(nameof(StringComparer.OrdinalIgnoreCase), BindingFlags.Public | BindingFlags.Static);

                Debug.Assert(stringComparerProperty != null);

                return Expression.Property(expression: null, stringComparerProperty);
            }

            PropertyInfo? equalityComparerProperty = typeof(EqualityComparer<>).MakeGenericType(type)
                .GetProperty("Default", BindingFlags.Public | BindingFlags.Static);

            Debug.Assert(equalityComparerProperty != null);

            return Expression.Property(expression: null, equalityComparerProperty);
        }
    }
}
