using Sharp.ValueObject.Visitors.Tests.Models;
using System.IO;

namespace Sharp.ValueObject.Visitors.Tests.Visitors
{
    internal class TextWriterColorVisitor : IColorVisitor
    {
        private readonly TextWriter _textWriter;

        public TextWriterColorVisitor(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public void VisitBlueColor()
        {
            _textWriter.WriteLine("Specific blue color handler");
        }

        public void VisitGreenColor()
        {
            _textWriter.WriteLine("Specific green color handler");
        }

        public void VisitRedColor()
        {
            _textWriter.WriteLine("Specific red color handler");
        }

        public void VisitUnknownColor(Color color)
        {
            _textWriter.WriteLine($"General {color} color handler");
        }
    }
}
