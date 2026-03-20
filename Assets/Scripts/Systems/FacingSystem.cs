using UnityEngine;

namespace FightTest.Systems
{
    public class FacingSystem : MonoBehaviour
    {
        public float Sign { get; private set; } = -1f;

        private void Awake()
        {
            ApplyScale();
        }

        public void SetFacing(float sign)
        {
            sign = sign >= 0f ? 1f : -1f;
            if (Mathf.Approximately(sign, Sign))
            {
                return;
            }

            Sign = sign;
            ApplyScale();
        }

        private void ApplyScale()
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * Sign;
            transform.localScale = s;
        }
    }
}