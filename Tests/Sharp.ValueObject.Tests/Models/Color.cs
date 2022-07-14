using System;

namespace Sharp.ValueObject.Tests.Models
{
    public sealed class Color : StringValueObject<Color>
    {
        public static Constant Red { get; } = new("red");
        public static Constant Green { get; } = new("green");
        public static Constant Blue { get; } = new("blue");

        public Color(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"The color name cannot be null or empty");
        }
    }
}
