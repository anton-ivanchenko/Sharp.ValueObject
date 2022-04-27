using Sharp.ValueObject.ValueHandlers.Tests.Models;

namespace Sharp.ValueObject.ValueHandlers.Tests.Handlers.Color
{
    internal class SimpleMessageColorHandler : IColorHandler<string>
    {
        public string HandleBlueColor()
        {
            return "Specific blue color handler";
        }

        public string HandleGreenColor()
        {
            return "Specific green color handler";
        }

        public string HandleRedColor()
        {
            return "Specific red color handler";
        }

        public string DefaultCase(Models.Color color)
        {
            return $"General {color} color handler";
        }
    }
}
