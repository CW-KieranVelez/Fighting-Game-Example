using UnityEngine;

namespace FightTest.Data
{
    [CreateAssetMenu(menuName = "FightTest/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [Header("Movement")]
        public float MoveSpeed = 4f;
        public float WalkBackSpeed = 2.5f;
        public float SprintSpeed = 8f;
        public float BackDashSpeed = 10f;
        public float BackDashDuration = 0.2f;
        public float JumpForce = 10f;

        [Header("Health")]
        public int MaxHealth = 100;
        
        [Header("Attack Data")]
        public AttackData ThrowAttack;
        public AttackData LightAttack;
        public AttackData HeavyAttack;
        public AttackData CrouchLightAttack;
        public AttackData CrouchHeavyAttack;
        public AttackData AirLightAttack;
        public AttackData AirHeavyAttack;
    }
}
