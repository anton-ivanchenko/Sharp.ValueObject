using System.Text.Json;

namespace Sharp.ValueObject.Json
{
    public class GenericValueObjectConverter<TValue, TValueObject> : ValueObjectConverter<TValue, TValueObject>
        where TValue : IEquatable<TValue>
        where TValueObject : ValueObject<TValue, TValueObject>
    {
        static GenericValueObjectConverter()
        {
            GenericValueObjectConverterImplementation<short>.SetMethods((ref Utf8JsonReader r) => r.GetInt16(), (w, v) => w.WriteNumberValue(v));
            GenericValueObjectConverterImplementation<int>.SetMethods((ref Utf8JsonReader r) => r.GetInt32(), (w, v) => w.WriteNumberValue(v));
            GenericValueObjectConverterImplementation<long>.SetMethods((ref Utf8JsonReader r) => r.GetInt64(), (w, v) => w.WriteNumberValue(v));

            GenericValueObjectConverterImplementation<float>.SetMethods((ref Utf8JsonReader r) => (float)r.GetDouble(), (w, v) => w.WriteNumberValue(v));
            GenericValueObjectConverterImplementation<double>.SetMethods((ref Utf8JsonReader r) => r.GetDouble(), (w, v) => w.WriteNumberValue(v));
            GenericValueObjectConverterImplementation<decimal>.SetMethods((ref Utf8JsonReader r) => r.GetDecimal(), (w, v) => w.WriteNumberValue(v));

            GenericValueObjectConverterImplementation<DateTime>.SetMethods((ref Utf8JsonReader r) => r.GetDateTime(), (w, v) => w.WriteStringValue(v));
            GenericValueObjectConverterImplementation<DateTimeOffset>.SetMethods((ref Utf8JsonReader r) => r.GetDateTimeOffset(), (w, v) => w.WriteStringValue(v));
        }

        protected override TValue? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (GenericValueObjectConverterImplementation<TValue>.TryReadValue(ref reader, out TValue? value))
            {
                return value;
            }

            return ThrowExceptionForNotSupportedType();
        }

        public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
        {
            if (!GenericValueObjectConverterImplementation<TValue>.TryWriteValue(writer, value.Value))
            {
                ThrowExceptionForNotSupportedType();
            }
        }

        private TValue? ThrowExceptionForNotSupportedType()
        {
            throw new InvalidOperationException($@"The generic value object converter does not support the type ""{typeof(TValue)}""");
        }
    }

    internal static class GenericValueObjectConverterImplementation<TValue>
    {
        public delegate TValue? ReadValueDelegate(ref Utf8JsonReader reader);
        public delegate void WriteValueDelegate(Utf8JsonWriter reader, TValue value);

        private static ReadValueDelegate? _readValue;
        private static WriteValueDelegate? _writeValue;

        public static void SetMethods(ReadValueDelegate readValue, WriteValueDelegate writeValue)
        {
            _readValue = readValue;
            _writeValue = writeValue;
        }

        public static bool TryReadValue(ref Utf8JsonReader reader, out TValue? value)
        {
            if (_readValue != null)
            {
                value = _readValue.Invoke(ref reader);
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryWriteValue(Utf8JsonWriter writer, TValue value)
        {
            if (_writeValue != null)
            {
                _writeValue.Invoke(writer, value);
                return true;
            }

            return false;
        }
    }
}
