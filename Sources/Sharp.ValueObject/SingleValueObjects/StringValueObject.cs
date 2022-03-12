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

        public static bool TryGetDeclaredConstantCaseInsensitive(string value, [NotNullWhen(true)] out Constant? constant)
            => TryGetDeclaredConstant(value, StringComparer.OrdinalIgnoreCase, out constant);

        public static bool TryGetDeclaredConstantCaseInsensitive<TConstant>(string value, [NotNullWhen(true)] out TConstant? constant)
            where TConstant : Constant => TryGetDeclaredConstant(value, StringComparer.OrdinalIgnoreCase, out constant);

        protected StringValueObject(string value) : base(value) { }
    }
}
