using System;
using UnityEngine;

namespace FightTest.Systems
{
    public class CharacterHealth : MonoBehaviour
    {
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> OnHealthChange;

        private void Awake()
        {
            Init(100);
        }

        public void Init(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}