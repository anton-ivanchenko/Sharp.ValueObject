namespace Sharp.ValueObject.Json.Tests.Models
{
    public sealed class Animal : SingleValueObject<string, Animal>
    {
        public static Constant Cat { get; } = new("Cat", code: 5);
        public static Constant Dog { get; } = new("Dog", code: 10);

        internal Animal(string value) : base(value) { }

        public new class Constant : SingleValueObject<string, Animal>.Constant
        {
            public Constant(string value, int code) : base(value)
            {
                Code = code;
            }

            public int Code { get; }
        }
    }
}
