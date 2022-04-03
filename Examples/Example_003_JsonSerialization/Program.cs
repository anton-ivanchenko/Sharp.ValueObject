using Example_003_JsonSerialization;
using Sharp.ValueObject.Json;
using System.Text.Json;

var options = new JsonSerializerOptions()
{
    Converters = { new ValueObjectConverterFactory() }
};
//=================================================================================================

var temperatureRange = new TemperatureRange()
{
    Min = new Temperature(-5.0F),
    Max = Temperature.WaterEvaporation
};

// Result json string: { "Min": -5.0, "Max": 100 }
string temperatureRangeJson = JsonSerializer.Serialize(temperatureRange, options);
Console.WriteLine($"Temperature json range: {temperatureRangeJson}");

TemperatureRange range = JsonSerializer.Deserialize<TemperatureRange>(temperatureRangeJson, options)!;
Console.WriteLine($"Temperature range ({range.Min}; {range.Max})");
//=================================================================================================

Temperature[] temperaturesArray = new[] { new Temperature(-10.5F), new Temperature(-2.3F), new Temperature(24.0F), new Temperature(125.5F) };

// Result json string: [-10.5, -2.3, 24, 125.5]
string temperatureArrayJson = JsonSerializer.Serialize(temperaturesArray, options);
Console.WriteLine($"Temperature json array: {temperatureArrayJson}");

List<Temperature> temperatures = JsonSerializer.Deserialize<List<Temperature>>(temperatureArrayJson, options)!;

Console.WriteLine();
Console.WriteLine($"{"Temperature",15}{"IsWaterFreeze",20}{"IsWaterEvaporation",25}");

foreach (var temperature in temperatures)
{
    Console.WriteLine($"{temperature,15}{temperature.IsWaterFreeze,20}{temperature.IsWaterEvaporation,25}");
}