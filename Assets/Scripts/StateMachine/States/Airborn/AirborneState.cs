using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class AirborneState : IState
    {
        private readonly ColliderSet _colliders;

        public AirborneState(ColliderSet colliders)
        {
            _colliders = colliders;
        }

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
