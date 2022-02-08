namespace Sharp.ValueObject.Json.Tests.Models
{
    public sealed class Number : ValueObject<int, Number>
    {
        public static Constant Zero { get; } = new(0);
        public static Constant One { get; } = new(1);
        public static Constant Two { get; } = new(2);
        public static Constant Three { get; } = new(3);

        internal Number(int value) : base(value) { }
    }
}
