using Example_002_ValueObject;

Console.WriteLine("Temperature constants:");

foreach (var declaredConstant in Temperature.DeclaredConstants
    .OfType<Temperature.Constant>()
    .OrderBy(c => c.Value))
{
    Console.WriteLine($"\t{declaredConstant.Name,-20}: {declaredConstant.Value:F2} degrees");
}

Console.WriteLine();
//=================================================================================================

float temperatureValue = 0.0F;

if (Temperature.TryGetDeclaredConstant(temperatureValue, out var declared) && declared is Temperature.Constant constant)
{
    Console.WriteLine($@"There is constant temperature ""{constant.Name}"" with value {constant.Value:F2} degrees");
}
else
{
    Console.WriteLine($"There is no constant temperature with value {temperatureValue:F2} degrees");
}

Console.WriteLine();
//=================================================================================================

Temperature temperature = new(temperatureValue);

if (temperature.IsWaterFreeze)
{
    Console.WriteLine($"At temperatures {temperature.Value} degrees the water freezes");
}
else if (temperature.IsWaterEvaporation)
{
    Console.WriteLine($"At temperatures {temperature.Value} degrees the water evaporations");
}
else
{
    Console.WriteLine($"The temperatures {temperature.Value} is normal for water");
}
