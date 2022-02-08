using Sharp.ValueObject.Tests.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class ValueObjectGenericTests
    {
        [Fact]
        public void GetDeclaredConstants_ReturnCollectionOfConstants()
        {
            var allConstants = new HashSet<Number.Constant>() {
                Number.Zero, Number.One, Number.Two, Number.Three
            };

            var declaredConstants = Number.GetDeclaredConstants().ToHashSet();

            Assert.Equal(allConstants, declaredConstants);
        }

        [Fact]
        public void GetDeclaredConstantsGeneric_ReturnCollectionOfConstants()
        {
            var allConstants = new HashSet<Animal.Constant>() {
                Animal.Cat, Animal.Dog
            };

            var declaredConstants = Animal.GetDeclaredConstants<Animal.Constant>().ToHashSet();

            Assert.Equal(allConstants, declaredConstants);
        }

        [Fact]
        public void TryGetDeclaredConstant_DefinedValue_ReturnTrueAndNotNullConstant()
        {
            int number = Number.Three.Value;

            bool tryResult = Number.TryGetDeclaredConstant(number, out Number.Constant? constant);

            Assert.True(tryResult);
            Assert.NotNull(constant);
            Assert.Equal(number, constant!.Value);
        }

        [Fact]
        public void TryGetDeclaredConstantGeneric_DefinedValue_ReturnTrueAndNotNullConstant()
        {
            string animalName = Animal.Cat.ToString();

            bool tryResult = Animal.TryGetDeclaredConstant(animalName, out Animal.Constant? constant);

            Assert.True(tryResult);
            Assert.NotNull(constant);
            Assert.Equal(animalName, constant!.Value);
        }

        [Fact]
        public void TryGetDeclaredConstant_NotDefinedValue_ReturnFalseAndNullConstant()
        {
            bool tryResult = Number.TryGetDeclaredConstant(int.MinValue, out Number.Constant? constant);

            Assert.False(tryResult);
            Assert.Null(constant);
        }

        [Fact]
        public void TryGetDeclaredValue_DefinedValue_ReturnTrueAndNotNullValue()
        {
            int number = Number.Three.Value;

            bool tryResult = Number.TryGetDeclaredValue(number, out Number? numberValueObject);

            Assert.True(tryResult);
            Assert.NotNull(numberValueObject);
            Assert.Equal(number, numberValueObject!.Value);
        }

        [Fact]
        public void TryGetDeclaredValue_NotDefinedValue_ReturnFalseAndNullValue()
        {
            bool tryResult = Number.TryGetDeclaredValue(int.MinValue, out Number? numberValueObject);

            Assert.False(tryResult);
            Assert.Null(numberValueObject);
        }

        [Fact]
        public void OperatorEqual_EqualValues_ReturnTrue()
        {
            Number left = Number.Zero;
            Number right = Number.Zero;

            Assert.True(left == right);
        }

        [Fact]
        public void OperatorEqual_NotEqualValues_ReturnFalse()
        {
            Number left = Number.Zero;
            Number right = Number.One;

            Assert.False(left == right);
        }

        [Fact]
        public void OperatorNotEqual_EqualValues_ReturnFalse()
        {
            Number left = Number.Zero;
            Number right = Number.Zero;

            Assert.False(left != right);
        }

        [Fact]
        public void OperatorNotEqual_NotEqualValues_ReturnTrue()
        {
            Number left = Number.Zero;
            Number right = Number.One;

            Assert.True(left != right);
        }

        [Fact]
        public void EqualsObject_EqualValueObject_ReturnTrue()
        {
            Number value = Number.One;
            object one = (Number)Number.One;

            Assert.True(value.Equals(one));
        }

        [Fact]
        public void EqualsObject_NotEqualValueObject_ReturnFalse()
        {
            Number value = Number.One;
            object one = (Number)Number.Two;

            Assert.False(value.Equals(one));
        }

        [Fact]
        public void EqualsObject_EqualConstant_ReturnTrue()
        {
            Number value = Number.One;
            object one = Number.One;

            Assert.True(value.Equals(one));
        }

        [Fact]
        public void EqualsObject_NotEqualConstant_ReturnFalse()
        {
            Number value = Number.One;
            object one = Number.Two;

            Assert.False(value.Equals(one));
        }

        [Fact]
        public void EqualsObject_SomeString_ReturnFalse()
        {
            Number value = Number.One;
            object one = "some string";

            Assert.False(value.Equals(one));
        }
    }
}
