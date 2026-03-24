using FightTest.StateMachine;
using FightTest.Systems;
using UnityEngine;

namespace FightTest.States
{
    public sealed class DashState : IState
    {
        private readonly ColliderSet _colliders;
        private readonly float _duration;
        private readonly CharacterMover _mover;
        private readonly float _speed;
        private float _lockedMoveX;
        private float _timer;

        public DashState(CharacterMover mover, float speed, float duration, ColliderSet colliders)
        {
            _mover = mover;
            _speed = speed;
            _duration = duration;
            _colliders = colliders;
        }

        public float MoveX { get; set; }
        public bool IsFinished { get; private set; }

        public void Enter()
        {
            IsFinished = false;
            _timer = 0f;
            _lockedMoveX = MoveX;
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            _mover.Move(_lockedMoveX, _speed);

            if (_timer >= _duration)
            {
                IsFinished = true;
            }
        }

        public void Exit()
        {
            IsFinished = false;
            _colliders.DisableSet();
        }
    }
}