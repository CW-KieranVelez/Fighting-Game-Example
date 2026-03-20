using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class KnockedDownState : IState
    {
        private const int KnockedDownTicks = 30;

        private readonly ColliderSet _colliders;
        private int _remainingTicks;

        public KnockedDownState(ColliderSet colliders)
        {
            _colliders = colliders;
        }

        public bool IsFinished { get; private set; }
        public bool IsInvulnerable => true;

        public void Enter()
        {
            IsFinished = false;
            _remainingTicks = KnockedDownTicks;
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
    }
}