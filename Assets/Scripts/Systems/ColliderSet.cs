using System;
using UnityEngine;

namespace FightTest.Systems
{
    public class ColliderSet : MonoBehaviour
    {
        [SerializeField] private Collider2D[] _hurtbox = Array.Empty<Collider2D>();
        [SerializeField] private Collider2D[] _hitboxes = Array.Empty<Collider2D>();

        public Collider2D[] Hurtboxes => _hurtbox;
        public Collider2D[] Hitboxes => _hitboxes;

        public void EnableSet()
        {
            gameObject.SetActive(true);
            DisableHitboxes();
        }

        public void DisableSet()
        {
            gameObject.SetActive(false);
        }

        public void EnableHitboxes()
        {
            foreach (var hitBoxes in _hitboxes)
            {
                hitBoxes.gameObject.SetActive(true);
            }
        }

        public void DisableHitboxes()
        {
            foreach (var hitBoxes in _hitboxes)
            {
                hitBoxes.gameObject.SetActive(false);
            }
        }
    }
}