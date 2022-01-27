using Example_001_StringEnum;

string directionString = "left";
//string directionString = "up";
//string directionString = "top";

if (!Direction.TryParseCaseInsensitive(directionString, out Direction? direction))
{
    Console.WriteLine("Wrong direction");
    return;
}

if (direction != Direction.Up)
{
    Console.WriteLine("go to up...");
    direction = Direction.Up;
}

Console.WriteLine($"Try move to the {direction.Value} direction");
