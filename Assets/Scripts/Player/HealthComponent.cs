using System;
using SlopJam.Combat;
using UnityEngine;

namespace SlopJam.Player
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 5;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsAlive => CurrentHealth > 0;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            NotifyHealthChanged();
        }

        public void SetMaxHealth(int value, bool healToFull = true)
        {
            maxHealth = Mathf.Max(1, value);
            if (healToFull)
            {
                CurrentHealth = maxHealth;
            }

            NotifyHealthChanged();
        }

        public void ApplyDamage(DamageRequest request)
        {
            if (!IsAlive)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - Mathf.Max(0, request.Amount));
            NotifyHealthChanged();

            if (CurrentHealth == 0)
            {
                OnDeath?.Invoke();
            }
        }

        private void NotifyHealthChanged()
        {
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}

