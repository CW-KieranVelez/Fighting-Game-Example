using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class HitStunState : IState
    {
        private readonly ColliderSet _colliders;
        private int _remainingTicks;

        public HitStunState(ColliderSet colliders)
        {
            _colliders = colliders;
        }

        public bool IsFinished { get; private set; }

        public void Enter()
        {
            IsFinished = false;
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _remainingTicks--;
            if (_remainingTicks <= 0)
            {
                IsFinished = true;
            }
        }

        public void Exit()
        {
            _colliders.DisableSet();
            IsFinished = false;
        }

        public void Configure(int ticks)
        {
            _remainingTicks = ticks;
        }
    }
}