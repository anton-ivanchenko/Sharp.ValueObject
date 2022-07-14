using Sharp.ValueObject.Tests.Models;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class StringValueObjectTests
    {
        [Fact]
        public void TryParse_DefinedValue_ReturnTrueAndNotNullValue()
        {
            string key = Color.Green.Value;

            bool tryResult = Color.TryParse(key, out Color? color);

            Assert.True(tryResult);
            Assert.NotNull(color);
            Assert.Equal(Color.Green, color);
        }

        [Fact]
        public void TryParse_NotDefinedValue_ReturnFalseAndNullValue()
        {
            bool tryResult = Color.TryParse("some string", out Color? color);

            Assert.False(tryResult);
            Assert.Null(color);
        }

        [Fact]
        public void TryParse_DefinedValueToLower_ReturnTrueAndNotNullValue()
        {
            string[] keys = { Color.Green.Value.ToLower(), Color.Green.Value.ToUpper() };

            foreach (var key in keys)
            {
                bool tryResult = Color.TryParse(key, out Color? color);

                Assert.True(tryResult);
                Assert.NotNull(color);
                Assert.Equal(Color.Green, color);
            }
        }

        [Fact]
        public void Equals_EqualValuesCaseInsensitive_ReturnTrue()
        {
            Color color = Color.Green;

            Assert.True(color.Equals(color.Value.ToLower()));
            Assert.True(color.Equals(color.Value.ToUpper()));
        }

        [Fact]
        public void Equals_NotEqualValuesCaseInsensitive_ReturnFalse()
        {
            Color color = Color.Green;

            Assert.False(color.Equals(Color.Red.Value.ToLower()));
            Assert.False(color.Equals(Color.Red.Value.ToUpper()));
        }

        [Fact]
        public void OperatorEquals_EqualValuesCaseInsensitive_ReturnTrue()
        {
            Color lowercaseColor = new(Color.Green.Value.ToLower());
            Color uppercaseColor = new(Color.Green.Value.ToUpper());

            Assert.True(lowercaseColor == Color.Green);
            Assert.True(uppercaseColor == Color.Green);
        }

        [Fact]
        public void OperatorEquals_NotEqualValuesCaseInsensitive_ReturnFalse()
        {
            Color lowercaseColor = new(Color.Green.Value.ToLower());
            Color uppercaseColor = new(Color.Green.Value.ToUpper());

            Assert.False(lowercaseColor == Color.Red);
            Assert.False(uppercaseColor == Color.Red);
        }
    }
}
