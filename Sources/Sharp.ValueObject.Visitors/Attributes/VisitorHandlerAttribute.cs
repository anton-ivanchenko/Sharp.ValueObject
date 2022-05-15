using System;

namespace Sharp.ValueObject.Visitors.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VisitorHandlerAttribute : Attribute
    {
        public VisitorHandlerAttribute()
        {
            ConstantName = string.Empty;
        }

        public VisitorHandlerAttribute(string constantName)
        {
            ConstantName = constantName;
        }

        public string ConstantName { get; }

        public bool IsDefaultHandler => string.Equals(ConstantName, string.Empty);
    }
}
