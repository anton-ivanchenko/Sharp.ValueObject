using System.Diagnostics;

namespace Sharp.ValueObject
{
    public abstract class ValueObject
    {
        public static bool IsValueObjectType(Type type)
            => typeof(ValueObject).IsAssignableFrom(type);

        public static Type GetInnerValueType(Type type)
            => GetGenericValueObjectType(type).GetGenericArguments()[0];

        public static Type GetGenericValueObjectType(Type type)
        {
            if (!IsValueObjectType(type))
            {
                throw new InvalidOperationException($@"The type ""{type}"" is not value object type");
            }

            Type current = type;

            while (current.BaseType != typeof(ValueObject))
            {
                Debug.Assert(current.BaseType != null);
                current = current.BaseType;
            }

            Debug.Assert(current.IsGenericType);
            Debug.Assert(current.GetGenericTypeDefinition() == typeof(SingleValueObject<,>));

            return current;
        }
    }
}
