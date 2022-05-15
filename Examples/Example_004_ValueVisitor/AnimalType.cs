using Sharp.ValueObject.SingleValueObjects;
using Sharp.ValueObject.Visitors;
using Sharp.ValueObject.Visitors.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Example_004_ValueVisitor
{
    public sealed class AnimalType : StringValueObject<AnimalType>
    {
        public static Constant Cat { get; } = new("cat");

        public static Constant Dog { get; } = new("dog");

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
        private AnimalType(string value) : base(value) { }
    }

    [VisitorTarget(typeof(AnimalType))]
    public interface IAnimalTypeVisitor : ISingleValueObjectVisitor<AnimalType>
    {
        [VisitorHandler(nameof(AnimalType.Cat))]
        void VisitCat();

        [VisitorHandler(nameof(AnimalType.Dog))]
        void VisitDog();

        void VisitOtherAnimalType(AnimalType animal);
    }
}
