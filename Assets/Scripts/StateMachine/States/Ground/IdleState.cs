using FightTest.StateMachine;
using FightTest.Systems;
using UnityEngine;

namespace FightTest.States
{
    public sealed class IdleState : IState
    {
        private readonly CharacterMover _mover;
        private readonly ColliderSet _colliders;

        public IdleState(CharacterMover mover, ColliderSet colliders)
        {
            _mover = mover;
            _colliders = colliders;
        }

        public void Enter()
        {
            _mover.Stop();
            _colliders.EnableSet();
        }

        public void Tick() { }

        public void Exit()
        {
            _colliders.DisableSet();
        }
    }
}
