using Example_004_ValueVisitor;
using Sharp.ValueObject.Visitors;

var storage = new Storage() { Cats = 7, Dogs = 4 };
Console.WriteLine($"Storage have {storage.Cats} cats and {storage.Dogs} dogs");

AnimalType[] animalTypes = new AnimalType[] { AnimalType.Dog, AnimalType.Dog, AnimalType.Cat };

var animalTypeHandler = new StorageAnimalTypeVisitor(storage);

foreach (var animalType in animalTypes)
{
    var animal = animalType.Accept(animalTypeHandler);
    Console.WriteLine($"The {animal.GetType().Name} was created");
}

Console.WriteLine($"Storage have {storage.Cats} cats and {storage.Dogs} dogs");
