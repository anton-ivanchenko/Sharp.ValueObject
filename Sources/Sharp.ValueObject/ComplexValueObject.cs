using Sharp.ValueObject.Internal.Reflection;
using System;
using System.Reflection;

namespace Sharp.ValueObject
{
    public interface IComplexValueObject { }

    public class ComplexValueObject<TValueObject> : ValueObject
        , IComplexValueObject
        , IEquatable<ComplexValueObject<TValueObject>>
        where TValueObject : ComplexValueObject<TValueObject>
    {
        private static readonly Func<TValueObject, TValueObject, bool> _equalityMethod;
        private static readonly Func<TValueObject, int> _hashcodeMethod;

        static ComplexValueObject()
        {
            const BindingFlags propertyMask = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

            var properties = typeof(TValueObject).GetProperties(propertyMask);
            _equalityMethod = ReflectionHelper.GenerateEqualityMethod<TValueObject>(properties);
            _hashcodeMethod = ReflectionHelper.GenerateHashcodeMethod<TValueObject>(properties);
        }

        public static bool operator ==(ComplexValueObject<TValueObject> left, ComplexValueObject<TValueObject> right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(ComplexValueObject<TValueObject> left, ComplexValueObject<TValueObject> right)
            => !(left == right);

        public bool Equals(ComplexValueObject<TValueObject>? other)
            => other is not null && _equalityMethod.Invoke((TValueObject)this, (TValueObject)other);

        public override bool Equals(object? obj)
            => Equals(obj as ComplexValueObject<TValueObject>);

        public override int GetHashCode()
            => _hashcodeMethod.Invoke((TValueObject)this);
    }
}
