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

        public void AddForce(Vector2 force)
        {
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
            _rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}