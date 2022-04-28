using Sharp.ValueObject.Internal;
using Sharp.ValueObject.Internal.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sharp.ValueObject
{
    public abstract partial class SingleValueObject<TValue, TValueObject> : ValueObject
        , ISingleValueObjectTrait
        , IEquatable<SingleValueObject<TValue, TValueObject>>
        , IEquatable<SingleValueObject<TValue, TValueObject>.Constant>
        , ICloneable
        where TValue : IEquatable<TValue>
        where TValueObject : SingleValueObject<TValue, TValueObject>
    {
        private static readonly bool _valueObjectFactoryIsPublic;
        private static readonly Func<TValue, TValueObject> _valueObjectFactory;
        private static readonly IReadOnlyCollection<Constant> _declaredConstants;

        static SingleValueObject()
        {
            _valueObjectFactory = ReflectionHelper.GenerateConstructorWithValueParameter<TValue, TValueObject>(out _valueObjectFactoryIsPublic);
            _declaredConstants = ReflectionHelper.ReflectConstants<TValue, TValueObject>();
        }

        public static IEnumerable<Constant> GetDeclaredConstants() => _declaredConstants;

        public static IEnumerable<TConstant> GetDeclaredConstants<TConstant>()
            where TConstant : Constant => _declaredConstants.OfType<TConstant>();

        public static bool TryGetDeclaredConstant(TValue value, [NotNullWhen(true)] out Constant? constant)
            => TryGetDeclaredConstant(value, EqualityComparer<TValue>.Default, out constant);

        public static bool TryGetDeclaredConstant(TValue value, IEqualityComparer<TValue> comparer, [NotNullWhen(true)] out Constant? constant)
        {
            constant = _declaredConstants.FirstOrDefault(c => comparer.Equals(c.Value, value));
            return constant is not null;
        }

        public static bool TryGetDeclaredConstant<TConstant>(TValue value, [NotNullWhen(true)] out TConstant? constant)
            where TConstant : Constant => TryGetDeclaredConstant(value, EqualityComparer<TValue>.Default, out constant);

        public static bool TryGetDeclaredConstant<TConstant>(TValue value, IEqualityComparer<TValue> comparer, [NotNullWhen(true)] out TConstant? constant)
            where TConstant : Constant
        {
            if (TryGetDeclaredConstant(value, comparer, out Constant? declaredConstant) && declaredConstant is TConstant typedConstant)
            {
                constant = typedConstant;
                return true;
            }

            constant = null;
            return false;
        }

        public static bool TryGetDeclaredValue(TValue value, [NotNullWhen(true)] out TValueObject? valueObject)
            => TryGetDeclaredValue(value, EqualityComparer<TValue>.Default, out valueObject);

        public static bool TryGetDeclaredValue(TValue value, IEqualityComparer<TValue> comparer, [NotNullWhen(true)] out TValueObject? valueObject)
        {
            if (TryGetDeclaredConstant(value, comparer, out Constant? constant))
            {
                valueObject = constant;
                return true;
            }

            valueObject = null;
            return false;
        }

        // TODO: This is probably not a necessary method.
        public static TValueObject Create(TValue value)
            => Create(value, EqualityComparer<TValue>.Default);

        // TODO: This is probably not a necessary method.
        public static TValueObject Create(TValue value, IEqualityComparer<TValue> comparer)
        {
            if (TryGetDeclaredValue(value, comparer, out TValueObject? valueObject))
            {
                return valueObject;
            }

            if (_valueObjectFactoryIsPublic)
            {
                return _valueObjectFactory.Invoke(value);
            }

            throw new InvalidOperationException($@"The value ""{value}"" cannot be used to create instance of {typeof(TValueObject)}");
        }

        public static bool operator ==(SingleValueObject<TValue, TValueObject>? left, SingleValueObject<TValue, TValueObject>? right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(SingleValueObject<TValue, TValueObject>? left, SingleValueObject<TValue, TValueObject>? right)
            => !(left == right);

        protected SingleValueObject(TValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TValue Value { get; }

        public override bool Equals(object? other)
        {
            if (other is TValue value)
                return Equals(value);

            if (other is SingleValueObject<TValue, TValueObject> valueObject)
                return Equals(valueObject);

            if (other is SingleValueObject<TValue, TValueObject>.Constant constant)
                return Equals(constant);

            return false;
        }

        public bool Equals(TValue? other)
            => other is not null && Value.Equals(other);

        public bool Equals(SingleValueObject<TValue, TValueObject>? other)
            => other is not null && Value.Equals(other.Value);

        public bool Equals(SingleValueObject<TValue, TValueObject>.Constant? other)
            => other is not null && Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString()!;

        public TValueObject Clone() => (TValueObject)((ICloneable)this).Clone();

        object ICloneable.Clone() => MemberwiseClone();

        public class Constant : IEquatable<SingleValueObject<TValue, TValueObject>>, IEquatable<Constant>
        {
            public static implicit operator TValueObject(Constant constant)
                => _valueObjectFactory.Invoke(constant.Value);

            public static bool operator ==(Constant? left, Constant? right)
                => left is null ? right is null : left.Equals(right);

            public static bool operator !=(Constant? left, Constant? right)
                => !(left == right);

            public static bool operator ==(Constant? left, SingleValueObject<TValue, TValueObject>? right)
                => left is null ? right is null : left.Equals(right);

            public static bool operator !=(Constant? left, SingleValueObject<TValue, TValueObject>? right)
                => !(left == right);

            public static bool operator ==(SingleValueObject<TValue, TValueObject>? left, Constant? right)
                => (right == left);

            public static bool operator !=(SingleValueObject<TValue, TValueObject>? left, Constant? right)
                => (right != left);

            public Constant(TValue value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public TValue Value { get; }

            public override bool Equals(object? obj)
            {
                if (obj is SingleValueObject<TValue, TValueObject> valueObject)
                    return Equals(valueObject);

                if (obj is SingleValueObject<TValue, TValueObject>.Constant constant)
                    return Equals(constant);

                return false;
            }

            public bool Equals(SingleValueObject<TValue, TValueObject>? other)
                => other is not null && Value.Equals(other.Value);

            public bool Equals(SingleValueObject<TValue, TValueObject>.Constant? other)
                => other is not null && Value.Equals(other.Value);

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => Value.ToString()!;
        }
    }
}
