using Sharp.ValueObject.Visitors.Attributes;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Sharp.ValueObject.Visitors
{
    public interface ISingleValueObjectVisitor<TValueObject>
        where TValueObject : ISingleValueObject
    {
    }

    public static class SingleValueObjectExtensions
    {
        public static void Accept<TValueObject, TVisitor>(this TValueObject valueObject, TVisitor visitor)
            where TValueObject : ISingleValueObject
            where TVisitor : notnull, ISingleValueObjectVisitor<TValueObject>
        {
            if (valueObject is null)
                throw new ArgumentNullException(nameof(valueObject));

            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            InternalImplementation<TValueObject>
                .GetMethod(visitor)
                .Invoke(valueObject, visitor);
        }

        private static class InternalImplementation<TValueObject>
            where TValueObject : ISingleValueObject
        {
            private static readonly ConcurrentDictionary<Type, Action<ISingleValueObject, object>> _functions;

            static InternalImplementation()
            {
                _functions = new ConcurrentDictionary<Type, Action<ISingleValueObject, object>>();
            }

            public static Action<ISingleValueObject, object> GetMethod<TVisitor>(TVisitor visitor)
                where TVisitor : notnull
            {
                Type visitorType = visitor.GetType();

                return _functions.GetOrAdd(visitorType, (Type type) =>
                {
                    var (attributeOwnerType, targetType) = FindVisitorBaseClass(visitorType);

                    if (attributeOwnerType == type)
                        return ReflectionHelper.GenerateVisitorMethod<TValueObject>(attributeOwnerType, targetType);

                    return _functions.GetOrAdd(attributeOwnerType,
                        (_) => ReflectionHelper.GenerateVisitorMethod<TValueObject>(attributeOwnerType, targetType));
                });
            }

            private static (Type attributeOwnerType, Type targetType) FindVisitorBaseClass(Type visitorType)
            {
                foreach (var interfaceType in visitorType.GetInterfaces())
                {
                    var targetAttribute = interfaceType.GetCustomAttribute<VisitorTargetAttribute>();

                    if (targetAttribute is not null)
                    {
                        return (interfaceType, targetAttribute.SingleValueObjectType);
                    }
                }

                Type type = visitorType;

                while (type != typeof(object))
                {
                    var targetAttribute = type.GetCustomAttribute<VisitorTargetAttribute>();

                    if (targetAttribute is not null)
                        return (type, targetAttribute.SingleValueObjectType);

                    type = type.BaseType!;
                }

                throw new ArgumentException($"The type {visitorType} cannot be used as visitor object");
            }
        }
    }
}
