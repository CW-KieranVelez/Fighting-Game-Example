namespace FightTest.Data
{
    public readonly struct InputFrame
    {
        public readonly float MoveX;
        public readonly float MoveY;
        public readonly bool Jump;
        public readonly bool Duck;
        public readonly bool LightAttack;
        public readonly bool HeavyAttack;
        public readonly bool Throw;
        public readonly bool Sprint;

        public InputFrame(float moveX, float moveY, bool light, bool heavy, bool sprint = false)
        {
            MoveX = moveX;
            MoveY = moveY;
            Jump = moveY > 0.5f;
            Duck = moveY < -0.5f;
            LightAttack = light && !heavy;
            HeavyAttack = heavy && !light;
            Throw = light && heavy;
            Sprint = sprint;
        }
    }
}