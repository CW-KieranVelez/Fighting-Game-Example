using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class AirKnockedDownState : IState
    {
        private readonly ColliderSet _colliders;

        public AirKnockedDownState(ColliderSet colliders)
        {
            _colliders = colliders;
        }

        public bool IsInvulnerable => true;

        public void Enter()
        {
            _colliders.EnableSet();
        }

        public void Tick() { }

        public void Exit()
        {
            _colliders.DisableSet();
        }
    }
}
