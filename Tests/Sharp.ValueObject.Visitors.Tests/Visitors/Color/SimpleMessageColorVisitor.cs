using Sharp.ValueObject.Visitors.Tests.Models;

namespace Sharp.ValueObject.Visitors.Tests.Visitors.Color
{
    internal class SimpleMessageColorVisitor : IColorVisitor<string>
    {
        public string VisitBlueColor()
        {
            return "Specific blue color handler";
        }

        public string VisitGreenColor()
        {
            return "Specific green color handler";
        }

        public string VisitRedColor()
        {
            return "Specific red color handler";
        }

        public string VisitUnknownColor(Models.Color color)
        {
            return $"General {color} color handler";
        }
    }
}
