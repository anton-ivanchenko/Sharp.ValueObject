namespace Sharp.ValueObject.ValueHandlers.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class MethodHandlerAttribute : Attribute
    {
        public MethodHandlerAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}
