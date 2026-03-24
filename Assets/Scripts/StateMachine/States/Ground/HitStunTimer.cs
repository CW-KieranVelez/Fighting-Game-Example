namespace FightTest.States
{
    public class HitStunTimer
    {
        public int RemainingTicks { get; private set; }
        public bool IsFinished => RemainingTicks <= 0;

        public void Configure(int ticks)
        {
            RemainingTicks = ticks;
        }

        public void Tick()
        {
            if (RemainingTicks > 0)
            {
                RemainingTicks--;
            }
        }
    }
}