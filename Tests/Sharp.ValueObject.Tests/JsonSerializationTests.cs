using Sharp.ValueObject.Json;
using Sharp.ValueObject.Tests.Models;
using System.Text.Json;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class JsonSerializationTests
    {
        private readonly JsonSerializerOptions _options;

        public JsonSerializationTests()
        {
            _options = new JsonSerializerOptions()
            {
                Converters = { new ValueObjectConverterFactory() }
            };
        }

        #region StringValueObject

        [Fact]
        public void SerializeStringValueObject_ExistsEntity_Successfuly()
        {
            Color color = Color.Red;
            string json = JsonSerializer.Serialize(color, _options);

            Assert.Equal($"\"{color.Value}\"", json);
        }

        [Fact]
        public void DeserializeStringValueObject_ExistsEntity_Successfuly()
        {
            Color expectedColor = Color.Green;

            string json = $"\"{expectedColor.Value}\"";
            Color? color = JsonSerializer.Deserialize<Color>(json, _options);

            Assert.Equal(expectedColor, color);
        }

        [Fact]
        public void DeserializeStringValueObject_CaseInsensitive_Successfuly()
        {
            Color expectedColor = Color.Green;
            string expectedColorString = expectedColor.ToString().ToUpper();

            string json = $"\"{expectedColorString}\"";
            Color? color = JsonSerializer.Deserialize<Color>(json, _options);

            Assert.Equal(expectedColor, color);
        }

        [Fact]
        public void DeserializeStringValueObject_NotExistsEntity_Successfuly()
        {
            string colorName = "not exists color";

            string json = $"\"{colorName}\"";
            Color? color = JsonSerializer.Deserialize<Color>(json, _options);

            Assert.NotNull(color);
            Assert.Equal(colorName, color!.Value);
        }

        [Fact]
        public void DeserializeStringValueObject_ValueObject_Successfuly()
        {
            Color expectedColor = Color.Green;

            string json = $@"{{ ""Value"": ""{expectedColor.Value}"" }}";
            Color? color = JsonSerializer.Deserialize<Color>(json, _options);

            Assert.NotNull(color);
            Assert.Equal(expectedColor, color);
        }

        #endregion

        #region IntValueObject

        [Fact]
        public void SerializeIntValueObject_ExistsEntity_Successfuly()
        {
            Number number = Number.Three;
            string json = JsonSerializer.Serialize(number, _options);

            Assert.Equal(number.ToString(), json);
        }

        [Fact]
        public void DeserializeIntValueObject_ExistsEntity_Successfuly()
        {
            Number expectedNumber = Number.Two;

            string json = $"{expectedNumber.Value}";
            Number? number = JsonSerializer.Deserialize<Number>(json, _options);

            Assert.NotNull(number);
            Assert.Equal(expectedNumber, number);
        }

        #endregion
    }
}
