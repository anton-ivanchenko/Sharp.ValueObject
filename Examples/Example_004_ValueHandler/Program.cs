using Example_004_ValueHandler;
using Sharp.ValueObject.ValueHandlers;

var storage = new Storage() { Cats = 7, Dogs = 4 };
Console.WriteLine($"Storage have {storage.Cats} cats and {storage.Dogs} dogs");

AnimalType[] animalTypes = new AnimalType[] { AnimalType.Dog, AnimalType.Dog, AnimalType.Cat };

var animalTypeHandler = new StorageCountHandler(storage);

foreach (var animalType in animalTypes)
{
    var animal = animalType.HandleValue(animalTypeHandler);
    Console.WriteLine($"The {animal.GetType().Name} was created");
}

Console.WriteLine($"Storage have {storage.Cats} cats and {storage.Dogs} dogs");
