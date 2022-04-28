using System;

namespace Sharp.ValueObject.ValueHandlers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InterfaceHandlerAttribute : Attribute
    {
        public InterfaceHandlerAttribute(Type handlerInterface, string defaultMethodName)
        {
            HandlerInterface = handlerInterface;
            DefaultMethodName = defaultMethodName;
        }

        public Type HandlerInterface { get; }
        public string DefaultMethodName { get; }
    }
}
