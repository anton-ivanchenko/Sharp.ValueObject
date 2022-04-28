using Sharp.ValueObject.Internal;
using System;
using System.Collections.Generic;

namespace Sharp.ValueObject
{
    public abstract class ValueObject
    {
        public static bool IsValueObjectType(Type type)
            => typeof(ValueObject).IsAssignableFrom(type);

        public static bool IsSingleValueObjectType(Type type)
            => typeof(ISingleValueObjectTrait).IsAssignableFrom(type);

        public static bool IsComplexValueObjectType(Type type)
            => typeof(IComplexValueObjectTrait).IsAssignableFrom(type);

        public static bool IsValueObjectCollectionType(Type type)
            => typeof(IEnumerable<ValueObject>).IsAssignableFrom(type);

        public static bool IsSingleValueObjectCollectionType(Type type)
            => typeof(IEnumerable<ISingleValueObjectTrait>).IsAssignableFrom(type);

        public static bool IsComplexValueObjectCollectionType(Type type)
            => typeof(IEnumerable<IComplexValueObjectTrait>).IsAssignableFrom(type);

        public static Type GetSingleValueObjectInnerValueType(Type type)
        {
            if (!IsSingleValueObjectType(type))
            {
                throw new InvalidOperationException($"The type {type} is not a single value object type");
            }

            return UnsafeGetGenericValueObjectType(type).GetGenericArguments()[0];
        }

        public static Type GetGenericValueObjectType(Type type)
        {
            if (!IsValueObjectType(type))
            {
                throw new InvalidOperationException($@"The type {type} is not a value object type");
            }

            return UnsafeGetGenericValueObjectType(type);
        }

        private static Type UnsafeGetGenericValueObjectType(Type type)
        {
            Type current = type;

            while (current.BaseType != typeof(ValueObject))
            {
                current = current.BaseType!;
            }

            return current;
        }
    }
}
