using System.Diagnostics.CodeAnalysis;

namespace Sharp.ValueObject
{
    public abstract class StringValueObject<TValueObject> : ValueObject<string, TValueObject>
        where TValueObject : ValueObject<string, TValueObject>
    {
        public static bool TryParse(string value, [NotNullWhen(true)] out TValueObject? instance)
            => TryGetDeclaredValue(value, out instance);

        public static bool TryParseCaseInsensitive(string value, [NotNullWhen(true)] out TValueObject? instance)
            => TryGetDeclaredValue(value, StringComparer.OrdinalIgnoreCase, out instance);

        protected StringValueObject(string value) : base(value) { }
    }
}
