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

// Result json string: { "Min": -5.0, "Max": 100 }
string temperatureRangeJson = JsonSerializer.Serialize(temperatureRange, options);
Console.WriteLine($"Temperature range: {temperatureRangeJson}");

TemperatureRange range = JsonSerializer.Deserialize<TemperatureRange>(temperatureRangeJson, options)!;
Console.WriteLine($"Temperature range from {range.Min} to {range.Max}");
