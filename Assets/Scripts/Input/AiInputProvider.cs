using FightTest.Data;

namespace FightTest.Input
{
    public class AiInputProvider : IInputProvider
    {
        public InputFrame GetFrame()
        {
            return new InputFrame(0f, 0f, false, false);
        }
    }
}
