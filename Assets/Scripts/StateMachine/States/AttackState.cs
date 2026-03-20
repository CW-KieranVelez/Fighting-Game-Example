using FightTest.Data;
using FightTest.StateMachine;
using FightTest.Systems;
using UnityEngine;

namespace FightTest.States
{
    public sealed class AttackState : IState
    {
        private readonly ColliderSet _colliders;
        private readonly AttackData _data;
        private readonly FacingSystem _facing;
        private readonly LayerMask _hitLayer;
        private readonly string _label;
        private readonly Collider2D[] _overlapBuffer = new Collider2D[8];
        private readonly GameObject _self;
        private bool _hasHitThisSwing;
        private float _timer;
        private bool _wasInActive;

        public AttackState(
            AttackData data,
            ColliderSet colliders,
            LayerMask hitLayer,
            FacingSystem facing,
            GameObject self,
            string label)
        {
            _data = data;
            _colliders = colliders;
            _hitLayer = hitLayer;
            _facing = facing;
            _self = self;
            _label = label;
        }

        public bool IsFinished { get; private set; }

        private float StartupDuration => _data.StartupFrames / 60f;
        private float ActiveDuration => _data.ActiveFrames / 60f;
        private float RecoveryDuration => _data.RecoveryFrames / 60f;
        private float TotalDuration => StartupDuration + ActiveDuration + RecoveryDuration;

        public void Enter()
        {
            IsFinished = false;
            _hasHitThisSwing = false;
            _wasInActive = false;
            _timer = 0f;
            _colliders.EnableSet();
        }

        public void Tick()
        {
            _timer += Time.deltaTime;

            var inActive = _timer >= StartupDuration && _timer < StartupDuration + ActiveDuration;

            if (inActive && !_wasInActive)
            {
                _colliders.EnableHitboxes();
                _wasInActive = true;
            }
            else if (!inActive && _wasInActive)
            {
                _colliders.DisableHitboxes();
            }

            if (inActive && !_hasHitThisSwing)
            {
                TryHit();
            }

            if (_timer >= TotalDuration)
            {
                IsFinished = true;
            }
        }

        public void Exit()
        {
            _colliders.DisableSet();
            IsFinished = false;
        }

        private void TryHit()
        {
            var filter = new ContactFilter2D();
            filter.SetLayerMask(_hitLayer);
            filter.useTriggers = true;

            foreach (var hitbox in _colliders.Hitboxes)
            {
                var count = hitbox.OverlapCollider(filter, _overlapBuffer);
                for (var i = 0; i < count; i++)
                {
                    if (_overlapBuffer[i].transform.IsChildOf(_self.transform))
                    {
                        continue;
                    }

                    var hittable = _overlapBuffer[i].GetComponentInParent<IHittable>();
                    if (hittable == null)
                    {
                        continue;
                    }

                    _hasHitThisSwing = true;
                    Debug.Log($"[{_label}] Hit {_overlapBuffer[i].name}");
                    hittable.ReceiveHit(_data);
                    return;
                }
            }
        }
    }
}