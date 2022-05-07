using Sharp.ValueObject.Visitors.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject.Visitors.Internal
{
    internal static class ReflectionHelper
    {
        public static Func<SingleValueObject<TValue, TValueObject>, ISingleValueObjectVisitor<TValueObject, TResult>, TResult> GenerateVisitMethod<TValue, TValueObject, TResult>()
            where TValue : IEquatable<TValue>
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            var valueObjectType = typeof(TValueObject);

            var valueObjectAttribute = valueObjectType
                .GetCustomAttribute<VisitorInterfaceAttribute>();

            if (valueObjectAttribute is null)
                throw new InvalidOperationException($"The type {valueObjectType} does not have declared interface handler type");

            Type handlerType = GetHandlerType<TResult>(valueObjectAttribute);

            var valueObjectVariable = Expression.Parameter(typeof(SingleValueObject<TValue, TValueObject>));
            var handlerVariable = Expression.Parameter(typeof(ISingleValueObjectVisitor<TValueObject, TResult>));

            var specificValueObjectVariable = Expression.Variable(valueObjectType);
            var specificHandlerVariable = Expression.Variable(handlerType);

            var exitLabel = Expression.Label(typeof(TResult));

            var constantMembers = valueObjectType
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => typeof(SingleValueObject<TValue, TValueObject>.Constant).IsAssignableFrom(p.PropertyType))
                .ToList();

            List<Expression> bodyExpressions = new(capacity: constantMembers.Capacity + 3)
            {
                Expression.Assign(specificValueObjectVariable, Expression.Convert(valueObjectVariable, valueObjectType)),
                Expression.Assign(specificHandlerVariable, Expression.Convert(handlerVariable, handlerType))
            };

            foreach (var constantMember in constantMembers)
            {
                var constantAttribute = constantMember
                    .GetCustomAttribute<VisitorMethodAttribute>();

                if (constantAttribute is null)
                    continue;

                var targetMethod = GetTargetMethod(handlerType, constantAttribute.MethodName);
                var targetMethodCall = CreateCallTargetMethodExpression(specificHandlerVariable, targetMethod, specificValueObjectVariable);

                var constantExpression = Expression.IfThen(
                    Expression.Equal(specificValueObjectVariable, Expression.Property(expression: null, constantMember)),
                    Expression.Return(exitLabel, targetMethodCall));

                bodyExpressions.Add(constantExpression);
            }

            var defaultTargetMethod = GetTargetMethod(handlerType, valueObjectAttribute.DefaultVisitorMethod);
            var defaultTargetMethodCall = CreateCallTargetMethodExpression(specificHandlerVariable, defaultTargetMethod, specificValueObjectVariable);

            bodyExpressions.Add(Expression.Label(exitLabel, defaultTargetMethodCall));

            var lambdaBody = Expression.Block(new[] { specificValueObjectVariable, specificHandlerVariable }, bodyExpressions);
            var lambdaMethod = Expression.Lambda<Func<SingleValueObject<TValue, TValueObject>, ISingleValueObjectVisitor<TValueObject, TResult>, TResult>>(lambdaBody, new[] { valueObjectVariable, handlerVariable });

            return lambdaMethod.Compile();
        }

        private static Type GetHandlerType<TResult>(VisitorInterfaceAttribute valueObjectAttribute)
        {
            var specifiedHandlerType = valueObjectAttribute.InterfaceType;

            var baseHandlerInterface = specifiedHandlerType.GetInterfaces()
                .FirstOrDefault(x => x.IsInterface && x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISingleValueObjectVisitor<,>));

            if (baseHandlerInterface == null)
                throw new InvalidOperationException($"The type {specifiedHandlerType} cannot be used as single value object handler");

            if (specifiedHandlerType.IsGenericType && !specifiedHandlerType.IsConstructedGenericType)
                return valueObjectAttribute.InterfaceType.MakeGenericType(typeof(TResult));

            return specifiedHandlerType;
        }

        private static Expression CreateCallTargetMethodExpression(ParameterExpression handler, MethodInfo targetMethod, ParameterExpression valueObject)
        {
            var parameters = targetMethod.GetParameters();

            if (parameters.Length == 0)
            {
                return Expression.Call(handler, targetMethod);
            }

            if (parameters.Length != 1)
                throw new InvalidOperationException($"The number of parameters of method {targetMethod} must be 1 of {valueObject.Type}");

            var valueObjectParameter = parameters[0];

            if (valueObjectParameter.ParameterType != valueObject.Type)
                throw new InvalidOperationException($"The number of parameters of method {targetMethod} must be 1 of {valueObject.Type}");

            return Expression.Call(handler, targetMethod, valueObject);
        }

        private static MethodInfo GetTargetMethod(Type handlerType, string methodName)
        {
            MethodInfo? method = handlerType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
                throw new InvalidOperationException($"There is no method with name {method} in {handlerType} type");

            return method;
        }
    }
}
