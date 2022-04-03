using Sharp.ValueObject;

namespace Example_003_JsonSerialization
{
    public class Temperature : SingleValueObject<float, Temperature>
    {
        public static Constant AbsoluteZero { get; } = new(-273.15F);
        public static Constant WaterFreeze { get; } = new(0.0F);
        public static Constant WaterEvaporation { get; } = new(100.0F);

        public Temperature(float value) : base(value)
        {
            if (value < AbsoluteZero.Value)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public bool IsWaterFreeze => Value <= WaterFreeze.Value;
        public bool IsWaterEvaporation => Value >= WaterEvaporation.Value;
    }
}
