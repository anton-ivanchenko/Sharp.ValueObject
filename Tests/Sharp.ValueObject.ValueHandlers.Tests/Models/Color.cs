using Sharp.ValueObject.ValueHandlers.Attributes;

namespace Sharp.ValueObject.ValueHandlers.Tests.Models
{
    [InterfaceHandler(typeof(IColorHandler<>), nameof(IColorHandler<object>.DefaultCase))]
    public class Color : SingleValueObject<string, Color>
    {
        public static Constant Transparent { get; } = new("transparent");

        [MethodHandler(nameof(IColorHandler<object>.HandleRedColor))]
        public static Constant Red { get; } = new("red");

        [MethodHandler(nameof(IColorHandler<object>.HandleGreenColor))]
        public static Constant Green { get; } = new("gree");

        [MethodHandler(nameof(IColorHandler<object>.HandleBlueColor))]
        public static Constant Blue { get; } = new("blue");

        public Color(string value) : base(value) { }
    }

    public interface IColorHandler<TResult> : ISingleValueObjectHandler<Color, TResult>
    {
        public TResult HandleRedColor();
        public TResult HandleGreenColor();
        public TResult HandleBlueColor();

        public TResult DefaultCase(Color color);
    }
}
