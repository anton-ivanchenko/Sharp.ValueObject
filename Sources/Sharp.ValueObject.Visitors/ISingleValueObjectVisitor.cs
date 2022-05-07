using Sharp.ValueObject.Visitors.Internal;
using System;

namespace Sharp.ValueObject.Visitors
{
    public interface ISingleValueObjectVisitor<TValueObject, TResult> { }

    public static class SingleValueObjectVisitorExtensions
    {
        public static TResult Accept<TValue, TValueObject, TResult>(
                this SingleValueObject<TValue, TValueObject> valueObject,
                ISingleValueObjectVisitor<TValueObject, TResult> handler)
            where TValue : IEquatable<TValue>
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            if (valueObject is null)
                throw new ArgumentNullException(nameof(valueObject));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            return InternalImplementation<TValue, TValueObject, TResult>.VisitMethod.Invoke(valueObject, handler);
        }

        private static class InternalImplementation<TValue, TValueObject, TResult>
            where TValue : IEquatable<TValue>
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            static InternalImplementation()
            {
                VisitMethod = ReflectionHelper.GenerateVisitMethod<TValue, TValueObject, TResult>();
            }

            public static Func<SingleValueObject<TValue, TValueObject>, ISingleValueObjectVisitor<TValueObject, TResult>, TResult> VisitMethod { get; }
        }
    }
}
