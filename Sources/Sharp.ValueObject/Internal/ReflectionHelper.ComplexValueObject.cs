using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject.Internal.Reflection
{
    internal static partial class ReflectionHelper
    {
        public static Func<TValueObject, TValueObject, bool> GenerateEqualityMethod<TValueObject>(PropertyInfo[] properties)
            where TValueObject : ComplexValueObject<TValueObject>
        {
            ParameterExpression left = Expression.Parameter(typeof(TValueObject), nameof(left));
            ParameterExpression right = Expression.Parameter(typeof(TValueObject), nameof(right));

            List<Expression> lambdaBodyMembers = new(capacity: properties.Length + 1);
            LabelTarget exitLabel = Expression.Label(typeof(bool));

            foreach (var property in properties)
            {
                // TODO: It may not be correct to compare values ​​if the type implements more than one IEquatable interface
                var propertyEquatableMethod = property.PropertyType.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEquatable<>))
                    .First(x => x.GetGenericArguments()[0].IsAssignableFrom(property.PropertyType))
                    .GetMethod(nameof(IEquatable<object>.Equals));

                Debug.Assert(propertyEquatableMethod is not null,
                    $"The type {property.PropertyType} does not implement compatible {typeof(IEquatable<>)}");

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

            var lambdaBody = Expression.Block(lambdaBodyMembers);
            var lambdaMethod = Expression.Lambda<Func<TValueObject?, TValueObject?, bool>>(lambdaBody, left, right);

            return lambdaMethod.Compile();
        }

        public static Func<TValueObject, int> GenerateHashcodeMethod<TValueObject>(PropertyInfo[] properties)
            where TValueObject : ComplexValueObject<TValueObject>
        {
            ParameterExpression instance = Expression.Parameter(typeof(TValueObject), nameof(instance));
            ParameterExpression hashcode = Expression.Variable(typeof(HashCode), nameof(hashcode));

            List<Expression> lambdaBodyMembers = new(capacity: properties.Length + 1);

            var addHashCodeMethod = typeof(HashCode).GetMethods()
                .First(x => x.Name == nameof(HashCode.Add) && x.GetParameters().Length == 1);

            foreach (var property in properties)
            {
                var genericAddHashCodeMethod = addHashCodeMethod.MakeGenericMethod(property.PropertyType);
                var callAddHashCode = Expression.Call(hashcode, genericAddHashCodeMethod, Expression.MakeMemberAccess(instance, property));

                lambdaBodyMembers.Add(callAddHashCode);
            }

            var getHashCodeMethod = typeof(HashCode).GetMethod(nameof(HashCode.ToHashCode), Array.Empty<Type>());

            Debug.Assert(getHashCodeMethod is not null,
                $"There is not compatible method {nameof(HashCode.ToHashCode)} in {typeof(HashCode)}");

            lambdaBodyMembers.Add(Expression.Call(hashcode, getHashCodeMethod));

            var lambdaBody = Expression.Block(variables: new[] { hashcode }, lambdaBodyMembers);
            var lambdaMethod = Expression.Lambda<Func<TValueObject, int>>(lambdaBody, instance);

            return lambdaMethod.Compile();
        }
    }
}
