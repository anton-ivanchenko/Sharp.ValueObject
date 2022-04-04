using Sharp.ValueObject.Internal;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject
{
    public class ComplexValueObject<TValueObject> : ValueObject
        , IComplexValueObjectTrait
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

            int lambdaBodyMembersCapacity = properties.Length + 1;
            List<Expression> lambdaBodyMembers = new(lambdaBodyMembersCapacity);
            LabelTarget exitLabel = Expression.Label(typeof(bool));

            foreach (var property in properties)
            {
                var propertyEquatableMethod = property.PropertyType
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEquatable<>))
                    .First(x => x.GetGenericArguments()[0].IsAssignableFrom(property.PropertyType))
                    .GetMethod("Equals");

                Debug.Assert(propertyEquatableMethod is not null);

                Expression? equalityExpression = null;

                if (property.PropertyType.IsClass)
                {
                    equalityExpression = Expression.IfThenElse(
                        Expression.Equal(Expression.MakeMemberAccess(left, property), Expression.Constant(null, property.PropertyType)),
                        Expression.IfThen(
                            Expression.NotEqual(Expression.MakeMemberAccess(right, property), Expression.Constant(null, property.PropertyType)),
                            Expression.Return(exitLabel, Expression.Constant(false))),
                        Expression.IfThen(Expression.Not(
                                Expression.Call(Expression.MakeMemberAccess(left, property), propertyEquatableMethod, Expression.MakeMemberAccess(right, property))),
                                Expression.Return(exitLabel, Expression.Constant(false))));
                }
                else
                {
                    equalityExpression = Expression.IfThen(Expression.Not(
                        Expression.Call(Expression.MakeMemberAccess(left, property), propertyEquatableMethod, Expression.MakeMemberAccess(right, property))),
                        Expression.Return(exitLabel, Expression.Constant(false)));
                }


                lambdaBodyMembers.Add(equalityExpression);
            }

            lambdaBodyMembers.Add(Expression.Label(exitLabel, Expression.Constant(true)));
            Debug.Assert(lambdaBodyMembers.Capacity == lambdaBodyMembersCapacity);

            var lambdaBody = Expression.Block(lambdaBodyMembers);
            var lambdaMethod = Expression.Lambda<Func<TValueObject?, TValueObject?, bool>>(lambdaBody, left, right);

            return lambdaMethod.Compile();
        }

        private static Func<TValueObject, int> CreateHashcodeMethod(PropertyInfo[] properties)
        {
            ParameterExpression instance = Expression.Parameter(typeof(TValueObject), nameof(instance));
            ParameterExpression hashcode = Expression.Variable(typeof(HashCode), nameof(hashcode));

            int lambdaBodyMembersCapacity = properties.Length + 1;
            List<Expression> lambdaBodyMembers = new(lambdaBodyMembersCapacity);

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
            Debug.Assert(lambdaBodyMembers.Capacity == lambdaBodyMembersCapacity);

            var lambdaBody = Expression.Block(variables: new[] { hashcode }, lambdaBodyMembers);
            var lambdaMethod = Expression.Lambda<Func<TValueObject, int>>(lambdaBody, instance);

            return lambdaMethod.Compile();
        }
    }
}
