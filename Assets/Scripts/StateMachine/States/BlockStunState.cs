using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class BlockStunState : IState
    {
        private readonly ColliderSet _colliders;
        private int _remainingTicks;

        public BlockStunState(ColliderSet colliders)
        {
            _colliders = colliders;
        }

        public bool IsFinished { get; private set; }

        public void Enter()
        {
            _colliders.EnableSet();
            IsFinished = false;
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

        public void Configure(int frames)
        {
            _remainingTicks = frames;
        }
    }
}