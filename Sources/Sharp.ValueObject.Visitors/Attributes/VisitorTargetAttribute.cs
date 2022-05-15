using System;

namespace Sharp.ValueObject.Visitors.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class VisitorTargetAttribute : Attribute
    {
        public VisitorTargetAttribute(Type singleValueObjectType)
        {
            SingleValueObjectType = singleValueObjectType;
        }

        public Type SingleValueObjectType { get; }
    }
}
