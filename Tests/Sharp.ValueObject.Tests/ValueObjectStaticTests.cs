using Sharp.ValueObject.Tests.Models;
using System;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class ValueObjectStaticTests
    {
        [Fact]
        public void IsValueObjectType_ValueObjectType_ReturnTrue()
        {
            Assert.True(ValueObject.IsValueObjectType(typeof(Color)));
        }

        [Fact]
        public void IsValueObjectType_NotValueObjectType_ReturnFalse()
        {
            Assert.False(ValueObject.IsValueObjectType(typeof(string)));
        }

        [Fact]
        public void GetInnerValueType_StringValueObject_ReturnString()
        {
            Assert.Equal(typeof(string), ValueObject.GetInnerValueType(typeof(Color)));
        }

        [Fact]
        public void GetInnerValueType_IntegerValueObject_ReturnInteger()
        {
            Assert.Equal(typeof(int), ValueObject.GetInnerValueType(typeof(Number)));
        }

        [Fact]
        public void GetGenericValueObjectType_StringValueObject_ReturnGenericValueObjectType()
        {
            Assert.Equal(typeof(SingleValueObject<string, Color>), ValueObject.GetGenericValueObjectType(typeof(Color)));
        }

        [Fact]
        public void GetGenericValueObjectType_IntegerValueObject_ReturnGenericValueObjectType()
        {
            Assert.Equal(typeof(SingleValueObject<int, Number>), ValueObject.GetGenericValueObjectType(typeof(Number)));
        }

        [Fact]
        public void GetGenericValueObjectType_String_ThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                ValueObject.GetGenericValueObjectType(typeof(string));
            });
        }
    }
}
