using Sharp.ValueObject.Visitors.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject.Visitors
{
    internal static class ReflectionHelper
    {
        private static ConstructorInfo _invalidOperationExceptionConstructor;
        private static MethodInfo _formatWithTwoParamsMethod;

        static ReflectionHelper()
        {
            var invalidOperationExceptionConstructor = typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) });
            Debug.Assert(invalidOperationExceptionConstructor is not null);

            _invalidOperationExceptionConstructor = invalidOperationExceptionConstructor;

            var formatWithTwoParamsMethod = typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object) });
            Debug.Assert(formatWithTwoParamsMethod is not null);

            _formatWithTwoParamsMethod = formatWithTwoParamsMethod;
        }

        public static Action<ISingleValueObject, object> GenerateVisitorMethod<TValueObject>(Type visitorType, Type targetType)
            where TValueObject : ISingleValueObject
        {
            var visitorParameter = Expression.Parameter(typeof(object));
            var valueObjectParameter = Expression.Parameter(typeof(ISingleValueObject));

            var visitorVariable = Expression.Variable(visitorType);
            var valueObjectVariable = Expression.Variable(targetType);

            MethodInfo[]? visitorMethods = visitorType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<Expression> bodyExpressions = new(capacity: visitorMethods.Length);

            bodyExpressions.Add(Expression.Assign(visitorVariable, Expression.Convert(visitorParameter, visitorVariable.Type)));
            bodyExpressions.Add(Expression.Assign(valueObjectVariable, Expression.Convert(valueObjectParameter, valueObjectVariable.Type)));

            LabelTarget? returnLabel = Expression.Label(typeof(void));
            MethodInfo? defaultMethod = null;

            foreach (var visitorMethod in visitorMethods)
            {
                var targetMethodAttribute = visitorMethod.GetCustomAttribute<VisitorHandlerAttribute>(inherit: true);

                if (targetMethodAttribute is null)
                    continue;

                if (targetMethodAttribute.IsDefaultHandler)
                {
                    defaultMethod = visitorMethod;
                    continue;
                }

                var targetConstant = targetType.GetProperty(targetMethodAttribute.ConstantName, BindingFlags.Public | BindingFlags.Static);

                if (targetConstant is null)
                    throw new InvalidOperationException();

                var condition = Expression.IfThen(
                    Expression.Equal(valueObjectVariable, Expression.Property(expression: null, targetConstant)),
                    Expression.Return(returnLabel, Expression.Call(visitorVariable, visitorMethod)));

                bodyExpressions.Add(condition);
            }

            if (defaultMethod is not null)
            {
                var defaultMethodExpression = Expression.Call(visitorVariable, defaultMethod, valueObjectVariable);
                bodyExpressions.Add(defaultMethodExpression);
            }
            else
            {
                var formatMethodExp = Expression.Call(_formatWithTwoParamsMethod,
                    Expression.Constant("The value \"{0}\" cannot be proccessed by {1}"), valueObjectParameter, visitorParameter);

                var exception = Expression.New(_invalidOperationExceptionConstructor, formatMethodExp);
                bodyExpressions.Add(Expression.Throw(exception));
            }

            bodyExpressions.Add(Expression.Label(returnLabel));

            var lambdaBody = Expression.Block(new[] { valueObjectVariable, visitorVariable }, bodyExpressions);
            var lambdaMethod = Expression.Lambda<Action<ISingleValueObject, object>>(lambdaBody, valueObjectParameter, visitorParameter);

            return lambdaMethod.Compile();
        }
    }
}
