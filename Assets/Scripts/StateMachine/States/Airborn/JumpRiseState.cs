using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class JumpRiseState : IState
    {
        private readonly ColliderSet _colliders;

        public JumpRiseState(ColliderSet colliders)
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
