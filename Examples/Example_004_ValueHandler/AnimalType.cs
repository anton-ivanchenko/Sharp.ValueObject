using Sharp.ValueObject;
using Sharp.ValueObject.ValueHandlers;
using Sharp.ValueObject.ValueHandlers.Attributes;

namespace Example_004_ValueHandler
{
    public interface IAnimalHandler<TResult> : ISingleValueObjectHandler<AnimalType, TResult>
    {
        TResult HandleCatCase();
        TResult HandleDogCase();

        TResult HandleGeneralCase(AnimalType animal);
    }

    [InterfaceHandler(typeof(IAnimalHandler<>), nameof(IAnimalHandler<object>.HandleGeneralCase))]
    public class AnimalType : SingleValueObject<string, AnimalType>
    {
        [MethodHandler(nameof(IAnimalHandler<object>.HandleCatCase))]
        public static Constant Cat { get; } = new("cat");

        [MethodHandler(nameof(IAnimalHandler<object>.HandleDogCase))]
        public static Constant Dog { get; } = new("dog");

        protected AnimalType(string value) : base(value) { }
    }
}