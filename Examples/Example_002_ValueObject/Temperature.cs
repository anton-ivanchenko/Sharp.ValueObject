using Sharp.ValueObject;

namespace Example_002_ValueObject
{
    public class Temperature : ValueObject<float, Temperature>
    {
        public static Constant AbsoluteZero { get; } = new("Absolute zero", -273.15F);
        public static Constant WaterFreeze { get; } = new("Water freeze", 0.0F);
        public static Constant WaterEvaporation { get; } = new("Water Evaporation", 100.0F);

        public Temperature(float value) : base(value)
        {
            if (value < AbsoluteZero.Value)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public bool IsWaterFreeze => Value <= WaterFreeze.Value;
        public bool IsWaterEvaporation => Value >= WaterEvaporation.Value;

        public new class Constant : ValueObject<float, Temperature>.Constant
        {
            public Constant(string name, float value) : base(value)
            {
                Name = name;
            }

            public string Name { get; set; }
        }
    }
}
