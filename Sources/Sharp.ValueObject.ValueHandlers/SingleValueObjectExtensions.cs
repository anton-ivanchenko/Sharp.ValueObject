using Sharp.ValueObject.ValueHandlers.Internal;

namespace Sharp.ValueObject.ValueHandlers
{
    public static class SingleValueObjectExtensions
    {
        public static TResult HandleValue<TValue, TValueObject, TResult>(this SingleValueObject<TValue, TValueObject> valueObject, ISingleValueObjectHandler<TValueObject, TResult> handler)
            where TValue : IEquatable<TValue>
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            ArgumentNullException.ThrowIfNull(valueObject, nameof(valueObject));
            ArgumentNullException.ThrowIfNull(handler, nameof(handler));

            return InternalImplementation<TValue, TValueObject, TResult>.HandlerMethodInvoker.Invoke(valueObject, handler);
        }

        private static class InternalImplementation<TValue, TValueObject, TResult>
            where TValue : IEquatable<TValue>
            where TValueObject : SingleValueObject<TValue, TValueObject>
        {
            private static readonly Lazy<Func<SingleValueObject<TValue, TValueObject>, ISingleValueObjectHandler<TValueObject, TResult>, TResult>> _methodInvoker;

            static InternalImplementation()
            {
                _methodInvoker = new Lazy<Func<SingleValueObject<TValue, TValueObject>, ISingleValueObjectHandler<TValueObject, TResult>, TResult>>(
                    valueFactory: ReflectionHelper.GenerateHandlerMethodInvoker<TValue, TValueObject, TResult>,
                    mode: LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public static Func<SingleValueObject<TValue, TValueObject>, ISingleValueObjectHandler<TValueObject, TResult>, TResult> HandlerMethodInvoker => _methodInvoker.Value;
        }
    }
}
