using FightTest.StateMachine;
using FightTest.Systems;
using UnityEngine;

namespace FightTest.States
{
    public sealed class BackDashState : IState
    {
        private readonly ColliderSet _colliders;
        private readonly float _duration;
        private readonly CharacterMover _mover;
        private readonly float _speed;
        private float _timer;

        public BackDashState(CharacterMover mover, float speed, float duration, ColliderSet colliders)
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
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            _mover.Move(MoveX, _speed);

            if (_timer >= _duration)
            {
                IsFinished = true;
            }
        }

        public void Exit()
        {
            _mover.Stop();
            IsFinished = false;
            _colliders.DisableSet();
        }
    }
}