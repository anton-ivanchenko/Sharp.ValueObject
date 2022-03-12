using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject
{
    public abstract partial class SingleValueObject<TValue, TValueObject> : ValueObject
        , IEquatable<SingleValueObject<TValue, TValueObject>>
        , IEquatable<SingleValueObject<TValue, TValueObject>.Constant>
        where TValue : IEquatable<TValue>
        where TValueObject : SingleValueObject<TValue, TValueObject>
    {
        private static readonly IReadOnlyCollection<Constant> _declaredConstants;

        static SingleValueObject()
        {
            var definedConstants = typeof(TValueObject)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                .Where(p => typeof(Constant).IsAssignableFrom(p.PropertyType))
                .Select(p => p.GetValue(null))
                .Cast<Constant>()
                .ToList();

            _declaredConstants = new ReadOnlyCollection<Constant>(definedConstants);
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
            if (other is SingleValueObject<TValue, TValueObject> valueObject)
                return Equals(valueObject);

            if (other is SingleValueObject<TValue, TValueObject>.Constant constant)
                return Equals(constant);

            return false;
        }

        public bool Equals(SingleValueObject<TValue, TValueObject>? other)
            => other is not null && Value.Equals(other.Value);

        public bool Equals(SingleValueObject<TValue, TValueObject>.Constant? other)
            => other is not null && Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString()!;

        public class Constant : IEquatable<SingleValueObject<TValue, TValueObject>>, IEquatable<Constant>
        {
            private static readonly Func<TValue, TValueObject> _valueObjectFactory;

            static Constant()
            {
                var constructorToInvoke = GetConstructorWithValueParameter();

                var valueParameter = Expression.Parameter(typeof(TValue));

                var factoryMethod = Expression.Lambda<Func<TValue, TValueObject>>(
                    Expression.New(constructorToInvoke, valueParameter),
                    valueParameter);

                _valueObjectFactory = factoryMethod.Compile();
            }

            private static ConstructorInfo GetConstructorWithValueParameter()
            {
                foreach (var constructor in typeof(TValueObject).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var parameters = constructor.GetParameters();

                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(TValue))
                        return constructor;
                }

                throw new ApplicationException($@"The type ""{typeof(TValueObject)}"" does not have constructor with single parameter of ""{typeof(TValue)}"" type");
            }

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
