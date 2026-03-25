using UnityEngine;

namespace FightTest.Data
{
    public enum AttackHeight
    {
        Mid,
        Low,
        Air
    }

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
        public int EnemyBlockStunFrames = 8;
        public Vector2 BlockKnockback;

        [Header("Lunge")]
        public Vector2 LungeForce;
        public int LungeFrame;

        [Header("Knockback")]
        public Vector2 Knockback;
        public bool KnocksDown;

        [Header("Damage")]
        public int Damage = 5;
    }
}