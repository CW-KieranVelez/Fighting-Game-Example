using UnityEngine;

namespace FightTest.Systems
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMover : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;

        public void Move(float directionX, float speed)
        {
            _rb.velocity = new Vector2(directionX * speed, _rb.velocity.y);
        }

        public void Stop()
        {
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
        }

        public void Jump(float force, float directionX = 0f)
        {
            _rb.velocity = new Vector2(directionX, force);
        }

        public void ApplyKnockback(float directionX, float force)
        {
            _rb.AddForce(new Vector2(-directionX * force, 0f), ForceMode2D.Impulse);
        }

        public void ApplyLaunch(float directionX, float forceX, float forceY)
        {
            _rb.AddForce(new Vector2(-directionX * forceX, forceY), ForceMode2D.Impulse);
        }
    }
}