using Sharp.ValueObject.Visitors.Attributes;

namespace Sharp.ValueObject.Visitors.Tests.Models
{
    public class Color : SingleValueObject<string, Color>
    {
        public static Constant Transparent { get; } = new("transparent");
        public static Constant Red { get; } = new("red");
        public static Constant Green { get; } = new("gree");
        public static Constant Blue { get; } = new("blue");

        public Color(string value) : base(value) { }
    }

    [VisitorTarget(typeof(Color))]
    public interface IColorVisitor : ISingleValueObjectVisitor<Color>
    {
        [VisitorHandler(nameof(Color.Red))]
        public void VisitRedColor();

        [VisitorHandler(nameof(Color.Green))]
        public void VisitGreenColor();

        [VisitorHandler(nameof(Color.Blue))]
        public void VisitBlueColor();

        [VisitorHandler]
        public void VisitUnknownColor(Color color);
    }
}
