namespace Example_004_ValueVisitor
{
    public class StorageAnimalTypeVisitor : IAnimalTypeVisitor<Animal>
    {
        private readonly Storage _storage;

        public StorageAnimalTypeVisitor(Storage storage)
        {
            _storage = storage;
        }

        public Animal VisitCat()
        {
            if (_storage.Cats <= 0)
            {
                ThrowNotEnoughException();
            }

            _storage.Cats -= 1;
            return new Cat();
        }

        public Animal VisitDog()
        {
            if (_storage.Dogs <= 0)
            {
                ThrowNotEnoughException();
            }

            _storage.Dogs -= 1;
            return new Dog();
        }

        public Animal VisitOtherAnimalType(AnimalType animal)
            => throw new InvalidOperationException($"Wrong animal type: {animal}");

        private void ThrowNotEnoughException()
            => throw new InvalidOperationException($"There is not enough items in storage");
    }
}