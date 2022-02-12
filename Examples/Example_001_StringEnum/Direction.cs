using Sharp.ValueObject.SingleValueObjects;

namespace Example_001_StringEnum
{
    public class Direction : StringValueObject<Direction>
    {
        public static Constant Up { get; } = new("Up");
        public static Constant Down { get; } = new("Down");
        public static Constant Left { get; } = new("Left");
        public static Constant Right { get; } = new("Right");

        protected Direction(string value) : base(value) { }
    }
}
