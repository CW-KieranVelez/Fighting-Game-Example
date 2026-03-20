using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class BlockState : IState
    {
        private readonly CharacterMover _mover;
        private readonly ColliderSet _colliders;
        private int _remainingTicks;

        public BlockState(CharacterMover mover, ColliderSet colliders)
        {
            _mover = mover;
            _colliders = colliders;
        }

        public bool IsBlocking { get; private set; }
        public bool IsFinished { get; private set; }

        public void Configure(int frames)
        {
            _remainingTicks = frames;
        }

        public void Enter()
        {
            _mover.Stop();
            _colliders.EnableSet();
            IsBlocking = true;
            IsFinished = false;
        }

        public void Tick()
        {
            _remainingTicks--;
            if (_remainingTicks <= 0)
                IsFinished = true;
        }

        public void Exit()
        {
            _colliders.DisableSet();
            IsBlocking = false;
            IsFinished = false;
        }
    }
}
