using System;

namespace Sharp.ValueObject.Visitors.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class VisitorMethodAttribute : Attribute
    {
        public VisitorMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}
