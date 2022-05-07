using Sharp.ValueObject.Visitors.Tests.Models;
using Sharp.ValueObject.Visitors.Tests.Visitors.Color;
using Xunit;

namespace Sharp.ValueObject.Visitors.Tests
{
    public class SingleValueObjectExtensionsTests
    {
        [Fact]
        public void HandleValue_SimpleMessageColorHandler_PropertyWithHandler_CallSpecificMethod()
        {
            Color color = Color.Blue;
            string messageText = color.Accept(new SimpleMessageColorVisitor());

            Assert.Equal("Specific blue color handler", messageText);
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_PropertyWithoutHandler_CallDefaultMethod()
        {
            Color color = Color.Transparent;
            string messageText = color.Accept(new SimpleMessageColorVisitor());

            Assert.Equal("General transparent color handler", messageText);
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_UnknownColor_CallDefaultMethod()
        {
            Color color = new("yellow");
            string messageText = color.Accept(new SimpleMessageColorVisitor());

            Assert.Equal("General yellow color handler", messageText);
        }
    }
}