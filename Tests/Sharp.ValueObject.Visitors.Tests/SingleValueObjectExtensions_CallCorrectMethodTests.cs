using Moq;
using Sharp.ValueObject.Visitors.Tests.Models;
using System.IO;
using Xunit;

namespace Sharp.ValueObject.Visitors.Tests
{
    public class SingleValueObjectExtensions_CallCorrectMethodTests
    {
        private class TextWriterColorVisitor : IColorVisitor
        {
            private readonly TextWriter _textWriter;

            public TextWriterColorVisitor(TextWriter textWriter) => _textWriter = textWriter;

            public void VisitBlueColor() => _textWriter.WriteLine("Specific blue color handler");

            public void VisitGreenColor() => _textWriter.WriteLine("Specific green color handler");

            public void VisitRedColor() => _textWriter.WriteLine("Specific red color handler");

            public void VisitUnknownColor(Color color) => _textWriter.WriteLine($"General {color} color handler");
        }

        private readonly Mock<TextWriter> _textWriter;
        private readonly TextWriterColorVisitor _visitor;

        public SingleValueObjectExtensions_CallCorrectMethodTests()
        {
            _textWriter = new Mock<TextWriter>();
            _visitor = new TextWriterColorVisitor(_textWriter.Object);
        }

        [Fact]
        public void Accept_ConstantWithHandler_CallSpecificMethod()
        {
            Color color = Color.Blue;
            color.Accept(_visitor);

            _textWriter.Verify(x => x.WriteLine("Specific blue color handler"));
        }

        [Fact]
        public void Accept_ConstantWithoutHandler_CallGeneralMethod()
        {
            Color color = Color.Transparent;
            color.Accept(_visitor);

            _textWriter.Verify(x => x.WriteLine("General transparent color handler"));
        }

        [Fact]
        public void Accept_ValueHandler_CallGeneralMethod()
        {
            Color color = new("yellow");
            color.Accept(_visitor);

            _textWriter.Verify(x => x.WriteLine("General yellow color handler"));
        }
    }
}
