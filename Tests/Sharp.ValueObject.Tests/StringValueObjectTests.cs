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
        public void TryParseCaseInsensitive_DefinedValue_ReturnTrueAndNotNullValue()
        {
            string[] keys = { Color.Green.Value.ToLower(), Color.Green.Value.ToUpper() };

            foreach (var key in keys)
            {
                bool tryResult = Color.TryParseCaseInsensitive(key, out Color? color);

                Assert.True(tryResult);
                Assert.NotNull(color);
                Assert.Equal(Color.Green, color);
            }
        }

        [Fact]
        public void TryParseCaseInsensitive_NotDefinedValue_ReturnFalseAndNullValue()
        {
            bool tryResult = Color.TryParseCaseInsensitive("some string", out Color? color);

            Assert.False(tryResult);
            Assert.Null(color);
        }
    }
}
