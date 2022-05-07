using Sharp.ValueObject.Visitors.Attributes;

namespace Sharp.ValueObject.Visitors.Tests.Models
{
    [VisitorInterface(typeof(IColorVisitor<>), nameof(IColorVisitor<object>.VisitUnknownColor))]
    public class Color : SingleValueObject<string, Color>
    {
        public static Constant Transparent { get; } = new("transparent");

        [VisitorMethod(nameof(IColorVisitor<object>.VisitRedColor))]
        public static Constant Red { get; } = new("red");

        [VisitorMethod(nameof(IColorVisitor<object>.VisitGreenColor))]
        public static Constant Green { get; } = new("gree");

        [VisitorMethod(nameof(IColorVisitor<object>.VisitBlueColor))]
        public static Constant Blue { get; } = new("blue");

        public Color(string value) : base(value) { }
    }

    public interface IColorVisitor<TResult> : ISingleValueObjectVisitor<Color, TResult>
    {
        public TResult VisitRedColor();
        public TResult VisitGreenColor();
        public TResult VisitBlueColor();

        public TResult VisitUnknownColor(Color color);
    }
}
