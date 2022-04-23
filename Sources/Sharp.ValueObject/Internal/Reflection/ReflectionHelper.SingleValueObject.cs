using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject.Internal.Reflection
{
    internal static partial class ReflectionHelper
    {
        public static IReadOnlyCollection<SingleValueObject<TValue, TValueObject>.Constant> ReflectConstants<TValue, TValueObject>()
            where TValue : IEquatable<TValue>
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
            where TValue : IEquatable<TValue>
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            const BindingFlags constructorMask = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var constructorToInvoke = typeof(TValueObject).GetConstructor(constructorMask, new[] { typeof(TValue) });

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
