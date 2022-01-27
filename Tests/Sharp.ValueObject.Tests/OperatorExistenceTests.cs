using Sharp.ValueObject.Tests.Models;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Sharp.ValueObject.Tests
{
    public class OperatorExistenceTests
    {
        [Fact]
        public void OperatorEquals_ConstantAndConstant_Exists()
        {
            Number.Constant left = Number.One;
            Number.Constant right = Number.One;

            Assert.True(ExistsOperator(left, right));
        }

        [Fact]
        public void OperatorEquals_ConstantAndValueObject_Exists()
        {
            Number.Constant left = Number.One;
            Number right = Number.One;

            Assert.True(ExistsOperator(left, right));
        }

        [Fact]
        public void OperatorEquals_ValueObjectAndConstant_Exists()
        {
            Number left = Number.One;
            Number.Constant right = Number.One;

            Assert.True(ExistsOperator(left, right));
        }

        private bool ExistsOperator<TValue1, TValue2>(TValue1 value1, TValue2 value2)
        {
            try
            {
                var lambda = Expression.Lambda<Func<bool>>(
                    Expression.Equal(
                        Expression.Constant(value1, typeof(TValue1)),
                        Expression.Constant(value2, typeof(TValue2))));

                lambda.Compile();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
