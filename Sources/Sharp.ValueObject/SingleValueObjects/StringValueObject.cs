using System.Diagnostics.CodeAnalysis;

namespace Sharp.ValueObject.SingleValueObjects
{
    public abstract class StringValueObject<TValueObject> : SingleValueObject<string, TValueObject>
        where TValueObject : SingleValueObject<string, TValueObject>
    {
        public static bool TryParse(string value, [NotNullWhen(true)] out TValueObject? instance)
            => TryGetDeclaredValue(value, out instance);

        public static bool TryParseCaseInsensitive(string value, [NotNullWhen(true)] out TValueObject? instance)
            => TryGetDeclaredValue(value, StringComparer.OrdinalIgnoreCase, out instance);

        protected StringValueObject(string value) : base(value) { }
    }
}
