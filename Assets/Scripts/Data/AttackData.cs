using UnityEngine;

namespace FightTest.Data
{
    public enum AttackHeight { Mid, Low, Air }

    [CreateAssetMenu(menuName = "FightTest/AttackData")]
    public class AttackData : ScriptableObject
    {
        [Header("Attack Properties")]
        public AttackHeight Height = AttackHeight.Mid;

        [Header("Frame Data")]
        public int StartupFrames = 4;
        public int ActiveFrames = 3;
        public int RecoveryFrames = 8;
        public int EnemyHitStopFrames = 4;

        [Header("Knockback")]
        public float Knockback = 2;
        public float KnockbackY = 0;
        public bool Launches = false;

        [Header("Damage")]
        public int Damage = 5;
    }
}