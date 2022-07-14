using System;
using System.Diagnostics.CodeAnalysis;

namespace Sharp.ValueObject
{
    public abstract class StringValueObject<TValueObject> : SingleValueObject<string, TValueObject>
        where TValueObject : SingleValueObject<string, TValueObject>
    {
        public static TValueObject Parse(string value)
        {
            if (!TryParse(value, out var valueObject))
                throw new InvalidOperationException($"The type {typeof(TValueObject)} cannot be parsed from {value} value");

            return valueObject;
        }

        public static TValueObject ParseOrCreate(string value)
        {
            if (!TryParseOrCreate(value, out var valueObject))
                throw new InvalidOperationException($"The type {typeof(TValueObject)} cannot be parsed from {value} value");

            return valueObject;
        }

        public static bool TryParse(string value, [NotNullWhen(true)] out TValueObject? instance)
            => TryGetDeclaredValue(value, out instance);

        public static bool TryParseOrCreate(string value, [NotNullWhen(true)] out TValueObject? instance)
        {
            if (TryParse(value, out instance))
            {
                return true;
            }

            if (HasPublicFactory)
            {
                instance = CreateUsingFactory(value);
                return true;
            }

            return false;
        }

        static StringValueObject()
        {
            SetEqualityComparer(StringComparer.OrdinalIgnoreCase);
        }

        protected StringValueObject(string value) : base(value) { }
    }
}
