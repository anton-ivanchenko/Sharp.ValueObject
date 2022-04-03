using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
            => ValueObject.IsSingleValueObjectType(typeToConvert) || TryGetEnumerableGenericParameter(typeToConvert, out _);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var factoryMethod = _cachedFactories.GetOrAdd(typeToConvert, t => CreateFactory(t));
            var jsonConverter = factoryMethod.Invoke();

            return jsonConverter;
        }

        private static Func<JsonConverter> CreateFactory(Type typeToConvert)
        {
            if (ValueObject.IsSingleValueObjectType(typeToConvert))
            {
                return CreateSingleValueObjectFactory(typeToConvert);
            }

            if (TryGetEnumerableGenericParameter(typeToConvert, out Type? valueType))
            {
                return CreateEnumerableValueObjectFactory(typeToConvert, valueType);
            }

            throw new InvalidOperationException(@$"This type of converter cannot be used to handle ""{typeToConvert}"" type");
        }

        private static Func<JsonConverter> CreateSingleValueObjectFactory(Type typeToConvert)
        {
            Type genericValueObjectType = ValueObject.GetGenericValueObjectType(typeToConvert);
            Type innerValueType = ValueObject.GetSingleValueObjectInnerValueType(genericValueObjectType);

            Type[] genericArguments = genericValueObjectType.GetGenericArguments();
            Type converterType = typeof(ValueObjectConverter<,>).MakeGenericType(genericArguments);

            ConstructorInfo? converterConstructor = converterType
                .GetConstructor(new[] { typeof(IEqualityComparer<>).MakeGenericType(innerValueType) });

            Debug.Assert(converterConstructor != null);

            var factoryMethod = Expression.Lambda<Func<JsonConverter>>(
                Expression.New(converterConstructor, GetEqualityComparer(innerValueType)));

            return factoryMethod.Compile();
        }

        private static Func<JsonConverter> CreateEnumerableValueObjectFactory(Type typeToConvert, Type valueType)
        {
            Type genericValueObjectType = ValueObject.GetGenericValueObjectType(valueType);
            Type[] genericArguments = genericValueObjectType.GetGenericArguments();

            Array.Resize(ref genericArguments, 3);
            genericArguments[2] = typeToConvert;

            Type converterType = typeof(EnumerableValueObjectConverter<,,>).MakeGenericType(genericArguments);
            ConstructorInfo? converterConstructor = converterType.GetConstructor(Array.Empty<Type>());

            Debug.Assert(converterConstructor != null);

            var factoryMethod = Expression.Lambda<Func<JsonConverter>>(Expression.New(converterConstructor));

            return factoryMethod.Compile();
        }

        private static bool TryGetEnumerableGenericParameter(Type typeToConvert, [NotNullWhen(true)] out Type? valueObjectType)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(typeToConvert))
            {
                valueObjectType = null;
                return false;
            }

            valueObjectType = typeToConvert.GetInterfaces()
                .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(type => type.GetGenericArguments()[0])
                .FirstOrDefault(type => ValueObject.IsSingleValueObjectType(type));

            return valueObjectType != null;
        }

        private static Expression GetEqualityComparer(Type type)
        {
            BindingFlags propertyBindingMask = BindingFlags.Public | BindingFlags.Static;

            // TODO: For strings, case-insensitive comparisons are always used
            PropertyInfo? equalityComparerProperty = (type == typeof(string))
                ? typeof(StringComparer).GetProperty(nameof(StringComparer.OrdinalIgnoreCase), propertyBindingMask)
                : typeof(EqualityComparer<>).MakeGenericType(type).GetProperty("Default", propertyBindingMask);

            Debug.Assert(equalityComparerProperty != null);

            return Expression.Property(expression: null, equalityComparerProperty);
        }
    }
}
