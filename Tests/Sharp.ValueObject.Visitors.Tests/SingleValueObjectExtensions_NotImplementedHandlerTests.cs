using Sharp.ValueObject.Visitors.Attributes;
using Sharp.ValueObject.Visitors.Tests.Models;
using System;
using Xunit;

namespace Sharp.ValueObject.Visitors.Tests
{
    public class SingleValueObjectExtensions_NotImplementedHandlerTests
    {
        [VisitorTarget(typeof(Color))]
        private class LastColorVisitor : ISingleValueObjectVisitor<Color>
        {
            public string? LastColor { get; private set; }

            [VisitorHandler(nameof(Color.Red))]
            public void HandleRedColor()
            {
                LastColor = "red";
            }
        }

        private readonly LastColorVisitor _visitor;

        public SingleValueObjectExtensions_NotImplementedHandlerTests()
        {
            _visitor = new();
        }

        [Fact]
        public void Accept_ConstantWithHandler_CallCorrectMethod()
        {
            Color color = Color.Red;
            color.Accept(_visitor);

            Assert.Equal("red", _visitor.LastColor);
        }

        [Fact]
        public void Accept_ConstantWithoutHandler_ThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Color color = Color.Blue;
                color.Accept(_visitor);
            });
        }
    }
}