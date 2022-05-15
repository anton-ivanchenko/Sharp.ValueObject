using Moq;
using Sharp.ValueObject.Visitors.Tests.Models;
using Sharp.ValueObject.Visitors.Tests.Visitors;
using System.IO;
using Xunit;

namespace Sharp.ValueObject.Visitors.Tests
{
    public class SingleValueObjectExtensionsTests
    {
        private readonly Mock<TextWriter> _textWriter;
        private readonly TextWriterColorVisitor _visitor;

        public SingleValueObjectExtensionsTests()
        {
            _textWriter = new Mock<TextWriter>();
            _visitor = new(_textWriter.Object);
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_PropertyWithHandler_CallSpecificMethod()
        {
            Color color = Color.Blue;
            color.Accept(_visitor);

            _textWriter.Verify(x => x.WriteLine("Specific blue color handler"));
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_PropertyWithoutHandler_CallDefaultMethod()
        {
            Color color = Color.Transparent;
            color.Accept(_visitor);

            _textWriter.Verify(x => x.WriteLine("General transparent color handler"));
        }

        [Fact]
        public void HandleValue_SimpleMessageColorHandler_UnknownColor_CallDefaultMethod()
        {
            Color color = new("yellow");
            color.Accept(_visitor);

            _textWriter.Verify(x => x.WriteLine("General yellow color handler"));
        }
    }
}