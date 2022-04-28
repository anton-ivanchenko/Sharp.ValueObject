namespace Example_004_ValueHandler
{
    public class StorageCountHandler : IAnimalHandler<Animal>
    {
        private readonly Storage _storage;

        public StorageCountHandler(Storage storage)
        {
            _storage = storage;
        }

        public Animal HandleCatCase()
        {
            if (_storage.Cats <= 0)
            {
                HandleNotEnoughCase();
            }

            _storage.Cats -= 1;
            return new Cat();
        }

        public Animal HandleDogCase()
        {
            if (_storage.Dogs <= 0)
            {
                HandleNotEnoughCase();
            }

            _storage.Dogs -= 1;
            return new Dog();
        }

        public Animal HandleGeneralCase(AnimalType animal)
        {
            throw new InvalidOperationException($"Wrong animal type: {animal}");
        }

        private void HandleNotEnoughCase()
        {
            throw new InvalidOperationException($"There is not enough items in storage");
        }
    }
}