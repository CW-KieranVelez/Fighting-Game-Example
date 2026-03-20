using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class WalkState : IState
    {
        private readonly CharacterMover _mover;
        private readonly ColliderSet _colliders;

        public WalkState(CharacterMover mover, float speed, ColliderSet colliders)
        {
            _mover = mover;
            Speed = speed;
            _colliders = colliders;
        }

        public float MoveX { get; set; }
        public float Speed { get; set; }

        public void Enter()
        {
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _mover.Move(MoveX, Speed);
        }

        public void Exit()
        {
            _mover.Stop();
            _colliders.DisableSet();
        }
    }
}
