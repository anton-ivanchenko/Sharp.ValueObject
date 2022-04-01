using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject
{
    public class ComplexValueObject<TValueObject> : ValueObject
        , IEquatable<ComplexValueObject<TValueObject>>
        where TValueObject : ComplexValueObject<TValueObject>
    {
        private static readonly Func<TValueObject, TValueObject, bool> _equalityMethod;
        private static readonly Func<TValueObject, int> _hashcodeMethod;

        static ComplexValueObject()
        {
            const BindingFlags propertyMask = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

            var properties = typeof(TValueObject).GetProperties(propertyMask);
            _equalityMethod = CreateEqualityMethod(properties);
            _hashcodeMethod = CreateHashcodeMethod(properties);
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

        private static Func<TValueObject, TValueObject, bool> CreateEqualityMethod(PropertyInfo[] properties)
        {
            ParameterExpression left = Expression.Parameter(typeof(TValueObject), nameof(left));
            ParameterExpression right = Expression.Parameter(typeof(TValueObject), nameof(right));
            Expression equalityOperators = Expression.Constant(true, typeof(bool));

            foreach (var property in properties)
            {
                var propertyEquatableMethod = typeof(IEquatable<>)
                    .MakeGenericType(property.PropertyType)
                    .GetMethod("Equals");

                Debug.Assert(propertyEquatableMethod is not null);

                equalityOperators = Expression.AndAlso(equalityOperators, Expression.Call(
                    Expression.MakeMemberAccess(left, property),
                    propertyEquatableMethod,
                    Expression.MakeMemberAccess(right, property)));
            }

            var lambdaMethod = Expression.Lambda<Func<TValueObject?, TValueObject?, bool>>(equalityOperators, left, right);

            return lambdaMethod.Compile();
        }

        private static Func<TValueObject, int> CreateHashcodeMethod(PropertyInfo[] properties)
        {
            ParameterExpression instance = Expression.Parameter(typeof(TValueObject), nameof(instance));
            ParameterExpression hashcode = Expression.Variable(typeof(HashCode), nameof(hashcode));

            List<Expression> lambdaBodyMembers = new(capacity: properties.Length + 1);

            var addHashCodeMethod = typeof(HashCode).GetMethods()
                .First(x => x.Name == "Add" && x.GetParameters().Length == 1);

            foreach (var property in properties)
            {
                var genericAddHashCodeMethod = addHashCodeMethod.MakeGenericMethod(property.PropertyType);

                var callAddHashCode = Expression.Call(hashcode, genericAddHashCodeMethod, Expression.MakeMemberAccess(instance, property));

                lambdaBodyMembers.Add(callAddHashCode);
            }

            var getHashCodeMethod = typeof(HashCode).GetMethod(nameof(HashCode.ToHashCode), Array.Empty<Type>());
            Debug.Assert(getHashCodeMethod is not null);

            lambdaBodyMembers.Add(Expression.Call(hashcode, getHashCodeMethod));

            var lambdaBody = Expression.Block(variables: new[] { hashcode }, lambdaBodyMembers);
            var lambdaMethod = Expression.Lambda<Func<TValueObject, int>>(lambdaBody, instance);

            return lambdaMethod.Compile();
        }
    }
}
