using Sharp.ValueObject.ValueHandlers.Tests.Handlers.Color;
using Sharp.ValueObject.ValueHandlers.Tests.Models;
using Xunit;

namespace Sharp.ValueObject.ValueHandlers.Tests
{
    public class SingleValueObjectExtensionsTests
    {
        [Fact]
        public void HandleValue_SimpleMessageColorHandler_PropertyWithHandler_CallSpecificMethod()
        {
            Color color = Color.Blue;
            string messageText = color.HandleValue(new SimpleMessageColorHandler());

            Assert.Equal("Specific blue color handler", messageText);
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_PropertyWithoutHandler_CallDefaultMethod()
        {
            Color color = Color.Transparent;
            string messageText = color.HandleValue(new SimpleMessageColorHandler());

            Assert.Equal("General transparent color handler", messageText);
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_UnknownColor_CallDefaultMethod()
        {
            Color color = new("yellow");
            string messageText = color.HandleValue(new SimpleMessageColorHandler());

            Assert.Equal("General yellow color handler", messageText);
        }
    }
}