using UnityEngine;

namespace FightTest.Systems
{
    [RequireComponent(typeof(FacingSystem))]
    public class OpponentTracker : MonoBehaviour
    {
        [SerializeField] private Transform _opponent;

        private FacingSystem _facing;

        private void Awake()
        {
            _facing = GetComponent<FacingSystem>();
        }

        private void Update()
        {
            if (_opponent == null)
            {
                return;
            }

            var dir = _opponent.position.x - transform.position.x;
            _facing.SetFacing(dir);
        }

        public void SetTarget(Transform opponent)
        {
            _opponent = opponent;
        }
    }
}