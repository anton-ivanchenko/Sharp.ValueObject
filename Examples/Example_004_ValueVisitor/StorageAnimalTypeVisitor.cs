namespace Example_004_ValueVisitor
{
    public class StorageAnimalTypeVisitor : IAnimalTypeVisitor
    {
        private readonly Storage _storage;
        private Animal? _animal;

        public StorageAnimalTypeVisitor(Storage storage)
        {
            _storage = storage;
            _animal = null;
        }

        public Animal ReleaseAnimal()
        {
            if (_animal is null)
                throw new InvalidOperationException($"");

            return Interlocked.Exchange(ref _animal, null);
        }

        public void VisitCat()
        {
            if (_storage.Cats <= 0)
            {
                ThrowNotEnoughException();
            }

            _storage.Cats -= 1;
            _animal = new Cat();
        }

        public void VisitDog()
        {
            if (_storage.Dogs <= 0)
            {
                ThrowNotEnoughException();
            }

            _storage.Dogs -= 1;
            _animal = new Dog();
        }

        public void VisitOtherAnimalType(AnimalType animal)
            => throw new InvalidOperationException($"Wrong animal type: {animal}");

        private static void ThrowNotEnoughException()
            => throw new InvalidOperationException($"There is not enough items in storage");
    }
}