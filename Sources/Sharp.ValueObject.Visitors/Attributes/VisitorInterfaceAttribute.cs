using System;

namespace Sharp.ValueObject.Visitors.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class VisitorInterfaceAttribute : Attribute
    {
        public VisitorInterfaceAttribute(Type interfaceType, string defaultVisitorMethod)
        {
            InterfaceType = interfaceType;
            DefaultVisitorMethod = defaultVisitorMethod;
        }

        public Type InterfaceType { get; }
        public string DefaultVisitorMethod { get; }
    }
}
