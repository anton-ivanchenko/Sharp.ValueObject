using Sharp.ValueObject.Tests.Models;
using System;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class ValueObjectStaticTests
    {
        [Fact]
        public void IsValueObjectType_SingleValueObjectType_ReturnTrue()
        {
            Assert.True(ValueObject.IsValueObjectType(typeof(Color)));
        }

        [Fact]
        public void IsValueObjectType_ComplexValueObjectType_ReturnTrue()
        {
            Assert.True(ValueObject.IsValueObjectType(typeof(Address)));
        }

        [Fact]
        public void IsValueObjectType_NotValueObjectType_ReturnFalse()
        {
            Assert.False(ValueObject.IsValueObjectType(typeof(string)));
        }

        [Fact]
        public void IsSingleValueObjectType_SingleValueObjectType_ReturnTrue()
        {
            Assert.True(ValueObject.IsSingleValueObjectType(typeof(Color)));
        }

        [Fact]
        public void IsSingleValueObjectType_ComplexValueObjectType_ReturnFalse()
        {
            Assert.False(ValueObject.IsSingleValueObjectType(typeof(Address)));
        }

        [Fact]
        public void IsSingleValueObjectType_NotValueObjectType_ReturnFalse()
        {
            Assert.False(ValueObject.IsSingleValueObjectType(typeof(string)));
        }

        [Fact]
        public void IsComplexValueObjectType_SingleValueObjectType_ReturnFalse()
        {
            Assert.False(ValueObject.IsComplexValueObjectType(typeof(Color)));
        }

        [Fact]
        public void IsComplexValueObjectType_ComplexValueObjectType_ReturnTrue()
        {
            Assert.True(ValueObject.IsComplexValueObjectType(typeof(Address)));
        }

        [Fact]
        public void IsComplexValueObjectType_NotValueObjectType_ReturnFalse()
        {
            Assert.False(ValueObject.IsComplexValueObjectType(typeof(string)));
        }

        [Fact]
        public void GetSingleValueObjectInnerValueType_StringValueObject_ReturnString()
        {
            Assert.Equal(typeof(string), ValueObject.GetSingleValueObjectInnerValueType(typeof(Color)));
        }

        [Fact]
        public void GetSingleValueObjectInnerValueType_IntegerValueObject_ReturnInteger()
        {
            Assert.Equal(typeof(int), ValueObject.GetSingleValueObjectInnerValueType(typeof(Number)));
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
        public void GetGenericValueObjectType_ComplexValueObject_ReturnGenericValueObjectType()
        {
            Assert.Equal(typeof(ComplexValueObject<Address>), ValueObject.GetGenericValueObjectType(typeof(Address)));
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
