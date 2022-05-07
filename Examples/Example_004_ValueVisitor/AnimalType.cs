using Sharp.ValueObject;
using Sharp.ValueObject.Visitors;
using Sharp.ValueObject.Visitors.Attributes;

namespace Example_004_ValueVisitor
{
    public interface IAnimalTypeVisitor<TResult> : ISingleValueObjectVisitor<AnimalType, TResult>
    {
        TResult VisitCat();
        TResult VisitDog();

        TResult VisitOtherAnimalType(AnimalType animal);
    }

    [VisitorInterface(typeof(IAnimalTypeVisitor<>), nameof(IAnimalTypeVisitor<object>.VisitOtherAnimalType))]
    public class AnimalType : SingleValueObject<string, AnimalType>
    {
        [VisitorMethod(nameof(IAnimalTypeVisitor<object>.VisitCat))]
        public static Constant Cat { get; } = new("cat");

        [VisitorMethod(nameof(IAnimalTypeVisitor<object>.VisitDog))]
        public static Constant Dog { get; } = new("dog");

        protected AnimalType(string value) : base(value) { }
    }
}
