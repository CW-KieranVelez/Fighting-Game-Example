using System.Collections.Generic;
using UnityEngine;

namespace FightTest.Systems
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class GroundDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] [Range(0f, 1f)] private float _minGroundDot = 0.65f;

        private readonly HashSet<Collider2D> _groundContacts = new HashSet<Collider2D>();

        public bool IsGrounded => _groundContacts.Count > 0;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!IsGroundLayer(col.collider))
            {
                return;
            }

            if (HasValidGroundNormal(col))
            {
                _groundContacts.Add(col.collider);
            }
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            _groundContacts.Remove(col.collider);
        }

        private void OnCollisionStay2D(Collision2D col)
        {
            if (!IsGroundLayer(col.collider))
            {
                return;
            }

            if (HasValidGroundNormal(col))
            {
                _groundContacts.Add(col.collider);
            }
            else
            {
                _groundContacts.Remove(col.collider);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var col = GetComponent<Collider2D>();
            if (col == null)
            {
                return;
            }

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            var b = col.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
#endif

        private bool IsGroundLayer(Collider2D col)
        {
            return (_groundLayer.value & (1 << col.gameObject.layer)) != 0;
        }

        private bool HasValidGroundNormal(Collision2D col)
        {
            for (var i = 0; i < col.contactCount; i++)
            {
                var normal = col.GetContact(i).normal;
                if (Vector2.Dot(normal, Vector2.up) >= _minGroundDot)
                {
                    return true;
                }
            }

            return false;
        }
    }
}