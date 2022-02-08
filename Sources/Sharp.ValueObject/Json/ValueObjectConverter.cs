using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp.ValueObject.Json
{
    public class ValueObjectConverter<TValue, TValueObject> : JsonConverter<TValueObject>
        where TValue : IEquatable<TValue>
        where TValueObject : ValueObject<TValue, TValueObject>
    {
        private readonly static Func<TValue, TValueObject>? _factory;

        static ValueObjectConverter()
        {
            var constructor = typeof(TValueObject).GetConstructor(new[] { typeof(TValue) });

            if (constructor != null)
            {
                var parameter = Expression.Parameter(typeof(TValue));

                var expression = Expression.Lambda<Func<TValue, TValueObject>>(
                    Expression.New(constructor, parameter), parameter);

                _factory = expression.Compile();
            }
        }

        public ValueObjectConverter() : this(EqualityComparer<TValue>.Default) { }

        public ValueObjectConverter(IEqualityComparer<TValue> equalityComparer)
        {
            EqualityComparer = equalityComparer;
        }

        public IEqualityComparer<TValue> EqualityComparer { get; }

        public override TValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // TODO: It is probably not necessary to remove all converters
                var withoutConverters = new JsonSerializerOptions(options);
                withoutConverters.Converters.Clear();

                return JsonSerializer.Deserialize<TValueObject>(ref reader, withoutConverters);
            }

            TValue? value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            return value != null
                ? ConvertToValueObject(value)
                : null;
        }

        public override void Write(Utf8JsonWriter writer, TValueObject valueObject, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, valueObject.Value);
        }

        protected virtual TValueObject? ConvertToValueObject(TValue value)
        {
            if (ValueObject<TValue, TValueObject>.TryGetDeclaredValue(value, EqualityComparer, out TValueObject? valueObject))
            {
                return valueObject;
            }

            if (_factory != null)
            {
                return _factory.Invoke(value);
            }

            throw new InvalidOperationException($@"The value ""{value}"" cannot be used for create object of ""{typeof(TValueObject)}"" type");
        }
    }
}
