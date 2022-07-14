using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject.Internal.Reflection
{
    internal static partial class ReflectionHelper
    {
        public static IReadOnlyCollection<SingleValueObject<TValue, TValueObject>.Constant> ReflectConstants<TValue, TValueObject>()
            where TValue : notnull
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            var constants = typeof(TValueObject)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                .Where(p => typeof(SingleValueObject<TValue, TValueObject>.Constant).IsAssignableFrom(p.PropertyType))
                .Select(p => p.GetValue(null))
                .Cast<SingleValueObject<TValue, TValueObject>.Constant>()
                .ToList();

            return new ReadOnlyCollection<SingleValueObject<TValue, TValueObject>.Constant>(constants);
        }

        public static Func<TValue, TValueObject> GenerateConstructorWithValueParameter<TValue, TValueObject>(out bool isPublic)
            where TValue : notnull
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            const BindingFlags constructorMask = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

#if NET6_0_OR_GREATER
            var constructorToInvoke = typeof(TValueObject).GetConstructor(constructorMask, new[] { typeof(TValue) });
#else
            var constructorToInvoke = typeof(TValueObject)
                .GetConstructors(constructorMask)
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(TValue);
                });
#endif

            Debug.Assert(constructorToInvoke is not null,
                $"The type {typeof(TValueObject)} does not have constructor with single {typeof(TValue)} parameter");

            var valueParameter = Expression.Parameter(typeof(TValue));

            var factoryMethod = Expression.Lambda<Func<TValue, TValueObject>>(
                Expression.New(constructorToInvoke, valueParameter),
                valueParameter);

            isPublic = constructorToInvoke.IsPublic;
            return factoryMethod.Compile();
        }
    }
}
