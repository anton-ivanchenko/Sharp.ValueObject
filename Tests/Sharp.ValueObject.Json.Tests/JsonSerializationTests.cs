using Sharp.ValueObject.Json.Tests.Models;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Sharp.ValueObject.Json.Tests
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

        #region Collections

        [Fact]
        public void Serialize_ArrayOfStringValueObjects_Successfully()
        {
            var colors = new Color[] { Color.Red, Color.Green, Color.Blue };
            string json = JsonSerializer.Serialize(colors, _options);

            Assert.Equal(@$"[""{Color.Red}"",""{Color.Green}"",""{Color.Blue}""]", json);
        }

        [Fact]
        public void Deserialize_ArrayOfStringValueObjects_Successfully()
        {
            string json = @$"[""RED"",""Green"",""blue""]";
            var colors = JsonSerializer.Deserialize<Color[]>(json, _options);

            Assert.NotNull(colors);
            Assert.True(colors!.Length == 3);
            Assert.Equal(Color.Red.Value, colors[0].Value);
            Assert.Equal(Color.Green.Value, colors[1].Value);
            Assert.Equal(Color.Blue.Value, colors[2].Value);
        }

        [Fact]
        public void Serialize_ListOfStringValueObjects_Successfully()
        {
            var colors = new List<Color> { Color.Red, Color.Green, Color.Blue };
            string json = JsonSerializer.Serialize(colors, _options);

            Assert.Equal(@$"[""{Color.Red}"",""{Color.Green}"",""{Color.Blue}""]", json);
        }

        [Fact]
        public void Deserialize_ListOfStringValueObjects_Successfully()
        {
            string json = @$"[""{Color.Red}"",""{Color.Green}"",""{Color.Blue}""]";
            var colors = JsonSerializer.Deserialize<List<Color>>(json, _options);

            Assert.NotNull(colors);
            Assert.True(colors!.Count == 3);
            Assert.Equal(Color.Red.Value, colors[0].Value);
            Assert.Equal(Color.Green.Value, colors[1].Value);
            Assert.Equal(Color.Blue.Value, colors[2].Value);
        }

        #endregion
    }
}
