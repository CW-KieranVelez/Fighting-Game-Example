using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class HitStunState : IState
    {
        private readonly ColliderSet _colliders;
        private readonly HitStunTimer _timer;

        public HitStunState(ColliderSet colliders, HitStunTimer timer)
        {
            _colliders = colliders;
            _timer = timer;
        }

        public bool IsFinished => _timer.IsFinished;

        public void Enter()
        {
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _timer.Tick();
        }

        public void Exit()
        {
            _colliders.DisableSet();
        }
    }
}