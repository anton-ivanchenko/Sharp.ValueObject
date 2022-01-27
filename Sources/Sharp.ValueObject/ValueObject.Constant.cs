using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject
{
    public abstract partial class ValueObject<TValue, TValueObject>
    {
        public class Constant : IEquatable<ValueObject<TValue, TValueObject>>, IEquatable<Constant>
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

            public static bool operator ==(Constant? left, TValueObject? right)
                => left is null ? right is null : left.Equals(right);

            public static bool operator !=(Constant? left, TValueObject? right)
                => !(left == right);

            public static bool operator ==(TValueObject? left, Constant? right)
                => (right == left);

            public static bool operator !=(TValueObject? left, Constant? right)
                => (right != left);

            public Constant(TValue value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public TValue Value { get; }

            public override bool Equals(object? obj)
            {
                if (obj is ValueObject<TValue, TValueObject> valueObject)
                    return Equals(valueObject);

                if (obj is ValueObject<TValue, TValueObject>.Constant constant)
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
}
