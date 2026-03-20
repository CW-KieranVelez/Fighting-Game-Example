using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class CrouchWalkState : IState
    {
        private readonly CharacterMover _mover;
        private readonly float _speed;
        private readonly ColliderSet _colliders;

        public CrouchWalkState(CharacterMover mover, float speed, ColliderSet colliders)
        {
            _mover = mover;
            _speed = speed;
            _colliders = colliders;
        }

        public float MoveX { get; set; }

        public void Enter()
        {
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _mover.Move(MoveX, _speed);
        }

        public void Exit()
        {
            _mover.Stop();
            _colliders.DisableSet();
        }
    }
}
