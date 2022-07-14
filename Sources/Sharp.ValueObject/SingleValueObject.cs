using Sharp.ValueObject.Internal.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sharp.ValueObject
{
    public interface ISingleValueObject { }

    public abstract partial class SingleValueObject<TValue, TValueObject> : ValueObject
        , ISingleValueObject
        , IEquatable<SingleValueObject<TValue, TValueObject>>
        , IEquatable<SingleValueObject<TValue, TValueObject>.Constant>
        , ICloneable
        where TValue : notnull
        where TValueObject : SingleValueObject<TValue, TValueObject>
    {
        private static readonly bool _valueObjectFactoryIsPublic;
        private static readonly Func<TValue, TValueObject> _valueObjectFactory;
        private static readonly IReadOnlyCollection<Constant> _declaredConstants;
        private static IEqualityComparer<TValue> _valueEqualityComparer;

        static SingleValueObject()
        {
            _valueObjectFactory = ReflectionHelper.GenerateConstructorWithValueParameter<TValue, TValueObject>(out _valueObjectFactoryIsPublic);
            _declaredConstants = ReflectionHelper.ReflectConstants<TValue, TValueObject>();
            _valueEqualityComparer = EqualityComparer<TValue>.Default;
        }

        protected static bool HasPublicFactory => _valueObjectFactoryIsPublic;

        public static TValueObject Create(TValue value)
        {
            if (!TryCreate(value, out var valueObject))
                throw new InvalidOperationException($"The type {typeof(TValueObject)} cannot be created with {value} value");

            return valueObject;
        }

        public static bool TryCreate(TValue value, [NotNullWhen(true)] out TValueObject? valueObject)
        {
            if (HasPublicFactory)
            {
                valueObject = CreateUsingFactory(value);
                return true;
            }

            return TryGetDeclaredValue(value, out valueObject);
        }

        public static IEnumerable<Constant> GetDeclaredConstants() => _declaredConstants;

        public static IEnumerable<TConstant> GetDeclaredConstants<TConstant>()
            where TConstant : Constant => _declaredConstants.OfType<TConstant>();

        public static bool TryGetDeclaredConstant(TValue value, [NotNullWhen(true)] out Constant? constant)
        {
            constant = _declaredConstants.FirstOrDefault(c => _valueEqualityComparer.Equals(c.Value, value));
            return constant is not null;
        }

        public static bool TryGetDeclaredConstant<TConstant>(TValue value, [NotNullWhen(true)] out TConstant? constant)
            where TConstant : Constant
        {
            if (TryGetDeclaredConstant(value, out Constant? declaredConstant) && declaredConstant is TConstant typedConstant)
            {
                constant = typedConstant;
                return true;
            }

            constant = null;
            return false;
        }

        public static bool TryGetDeclaredValue(TValue value, [NotNullWhen(true)] out TValueObject? valueObject)
        {
            if (TryGetDeclaredConstant(value, out Constant? constant))
            {
                valueObject = constant;
                return true;
            }

            valueObject = null;
            return false;
        }

        public static bool operator ==(SingleValueObject<TValue, TValueObject>? left, SingleValueObject<TValue, TValueObject>? right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(SingleValueObject<TValue, TValueObject>? left, SingleValueObject<TValue, TValueObject>? right)
            => !(left == right);

        protected static TValueObject CreateUsingFactory(TValue value)
        {
            return _valueObjectFactory.Invoke(value);
        }

        protected static void SetEqualityComparer(IEqualityComparer<TValue> equalityComparer)
        {
            _valueEqualityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
        }

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
            => _valueEqualityComparer.Equals(Value, other);

        public bool Equals(SingleValueObject<TValue, TValueObject>? other)
            => other is not null && _valueEqualityComparer.Equals(Value, other.Value);

        public bool Equals(SingleValueObject<TValue, TValueObject>.Constant? other)
            => other is not null && _valueEqualityComparer.Equals(Value, other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString()!;

        public TValueObject Clone() => (TValueObject)((ICloneable)this).Clone();

        object ICloneable.Clone() => MemberwiseClone();

        public class Constant : IEquatable<SingleValueObject<TValue, TValueObject>>, IEquatable<Constant>
        {
            public static implicit operator TValueObject(Constant constant)
                => CreateUsingFactory(constant.Value);

            public static bool operator ==(Constant? left, Constant? right)
                => left is null ? right is null : left.Equals(right);

            public static bool operator !=(Constant? left, Constant? right)
                => !(left == right);

            public static bool operator ==(Constant? left, SingleValueObject<TValue, TValueObject>? right)
                => left is null ? right is null : right is not null && _valueEqualityComparer.Equals(left.Value, right.Value);

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
                => other is not null && _valueEqualityComparer.Equals(Value, other.Value);

            public bool Equals(SingleValueObject<TValue, TValueObject>.Constant? other)
                => other is not null && _valueEqualityComparer.Equals(Value, other.Value);

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => Value.ToString()!;
        }
    }
}
