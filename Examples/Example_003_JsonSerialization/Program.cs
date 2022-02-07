using Example_003_JsonSerialization;
using Sharp.ValueObject.Json;
using System.Text.Json;

var options = new JsonSerializerOptions()
{
    Converters = { new ValueObjectConverterFactory() }
};

var temperatureRange = new TemperatureRange()
{
    Min = new Temperature(-5.0F),
    Max = Temperature.WaterEvaporation
};

string temperatureRangeJson = JsonSerializer.Serialize(temperatureRange, options);
Console.WriteLine($"Temperature range: {temperatureRangeJson}");

TemperatureRange range = JsonSerializer.Deserialize<TemperatureRange>(temperatureRangeJson, options)!;
Console.WriteLine($"Temperature range from {range.Min} to {range.Max}");
