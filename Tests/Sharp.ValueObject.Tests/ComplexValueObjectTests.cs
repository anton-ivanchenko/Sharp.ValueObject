using Sharp.ValueObject.Tests.Models;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class ComplexValueObjectTests
    {
        [Fact]
        public void Equals_EqualsFieldValues_ReturnTrue()
        {
            var address1 = new Address("UA", "Kyiv", 0);
            var address2 = new Address("UA", "Kyiv", 0);

            Assert.Equal(address1, address2);
        }

        [Fact]
        public void Equals_NotEqualsFieldValues_ReturnFalse()
        {
            var address1 = new Address("UA", "Kyiv", 0);
            var address2 = new Address("FR", "Paris", 0);

            Assert.NotEqual(address1, address2);
        }

        [Fact]
        public void Equals_EqualComplexValues_ReturnTrue()
        {
            var person1 = new Person(new("Alex", "Gordon"), new(25), "empty");
            var person2 = new Person(new("Alex", "Gordon"), new(25), "empty");

            Assert.Equal(person1, person2);
        }

        [Fact]
        public void Equals_NotEqualComplexValues_ReturnFalse()
        {
            var person1 = new Person(new("Alex", "Novicov"), new(25), "empty");
            var person2 = new Person(new("Alex", "Gordon"), new(25), "empty");

            Assert.NotEqual(person1, person2);
        }

        [Fact]
        public void Equals_BothHaveNullValue_NoException()
        {
            var address1 = new Address("UA", null, 0);
            var address2 = new Address("UA", null, 0);

            Assert.Equal(address1, address2);
        }

        [Fact]
        public void Equals_FirstHaveNullValue_NoException()
        {
            var address1 = new Address("UA", null, 0);
            var address2 = new Address("UA", "Paris", 0);

            Assert.NotEqual(address1, address2);
        }

        [Fact]
        public void Equals_SecondHaveNullValue_NoException()
        {
            var address1 = new Address("UA", "Kyiv", 0);
            var address2 = new Address("UA", null, 0);

            Assert.NotEqual(address1, address2);
        }

        [Fact]
        public void GetHashCode_EqualsFieldValues_ReturnEqualsHashCode()
        {
            var address1 = new Address("UA", "Kyiv", 0);
            var address2 = new Address("UA", "Kyiv", 0);

            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_NotEqualsFieldValues_ReturnNotEqualsHashCode()
        {
            var address1 = new Address("UA", "Kyiv", 0);
            var address2 = new Address("FR", "Paris", 0);

            Assert.NotEqual(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_HaveNullValue_NoException()
        {
            var address1 = new Address("UA", null, 0);
            var address2 = new Address("UA", null, 0);

            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }
    }
}
