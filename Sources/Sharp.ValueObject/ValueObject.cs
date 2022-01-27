﻿using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Sharp.ValueObject
{
    public abstract partial class ValueObject<TValue, TValueObject>
        : IEquatable<ValueObject<TValue, TValueObject>>
        , IEquatable<ValueObject<TValue, TValueObject>.Constant>
        where TValue : IEquatable<TValue>
        where TValueObject : ValueObject<TValue, TValueObject>
    {
        static ValueObject()
        {
            var definedConstants = typeof(TValueObject)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                .Where(p => typeof(Constant).IsAssignableFrom(p.PropertyType))
                .Select(p => p.GetValue(null))
                .Cast<Constant>()
                .ToList();

            DeclaredConstants = new ReadOnlyCollection<Constant>(definedConstants);
        }

        public static IReadOnlyCollection<Constant> DeclaredConstants { get; }

        public static bool TryGetDeclaredConstant(TValue value, [NotNullWhen(true)] out Constant? constant)
            => TryGetDeclaredConstant(value, EqualityComparer<TValue>.Default, out constant);

        public static bool TryGetDeclaredConstant(TValue value, IEqualityComparer<TValue> comparer, [NotNullWhen(true)] out Constant? constant)
        {
            constant = DeclaredConstants.FirstOrDefault(c => comparer.Equals(c.Value, value));
            return constant is not null;
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

        public static bool operator ==(ValueObject<TValue, TValueObject>? left, ValueObject<TValue, TValueObject>? right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(ValueObject<TValue, TValueObject>? left, ValueObject<TValue, TValueObject>? right)
            => !(left == right);

        protected ValueObject(TValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TValue Value { get; }

        public override bool Equals(object? other)
        {
            if (other is ValueObject<TValue, TValueObject> valueObject)
                return Equals(valueObject);

            if (other is ValueObject<TValue, TValueObject>.Constant constant)
                return Equals(constant);

            return false;
        }

        public bool Equals(ValueObject<TValue, TValueObject>? other)
            => other is not null && Value.Equals(other.Value);

        public bool Equals(ValueObject<TValue, TValueObject>.Constant? other)
            => other is not null && Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();
    }
}
