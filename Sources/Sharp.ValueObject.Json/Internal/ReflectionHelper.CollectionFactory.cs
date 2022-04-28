using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharp.ValueObject.Json.Internal
{
    internal static partial class ReflectionHelper
    {
        public static Func<IEnumerable<TValueObject?>, TCollection> GenerateCollectionFactory<TValueObject, TCollection>()
        {
            return typeof(TCollection).IsArray
                ? GenerateEnumerableArrayFactory()
                : GenerateEnumerableCollectionFactory();

            static Func<IEnumerable<TValueObject?>, TCollection> GenerateEnumerableArrayFactory()
            {
                MethodInfo toArrayMethod = typeof(Enumerable)
                    .GetMethod(nameof(Enumerable.ToArray))
                    !.MakeGenericMethod(typeof(TValueObject));

                var parameter = Expression.Parameter(typeof(IEnumerable<TValueObject>));

                var expression = Expression.Lambda<Func<IEnumerable<TValueObject?>, TCollection>>(
                    Expression.Call(toArrayMethod, parameter), parameter);

                return expression.Compile();
            }

            static Func<IEnumerable<TValueObject?>, TCollection> GenerateEnumerableCollectionFactory()
            {
                ConstructorInfo? collectionConstructor = typeof(TCollection)
                    .GetConstructor(new[] { typeof(IEnumerable<TValueObject>) });

                Debug.Assert(collectionConstructor is not null,
                    $"The collection {typeof(TCollection)} does not have constructor with {typeof(IEnumerable<TValueObject>)} parameter");

                var parameter = Expression.Parameter(typeof(IEnumerable<TValueObject>));

                var expression = Expression.Lambda<Func<IEnumerable<TValueObject?>, TCollection>>(
                    Expression.New(collectionConstructor, parameter), parameter);

                return expression.Compile();
            }
        }
    }
}
