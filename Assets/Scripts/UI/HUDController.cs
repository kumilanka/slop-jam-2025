using SlopJam.Player;
using UnityEngine;
using UnityEngine.Events;

namespace SlopJam.UI
{
    public class HUDController : MonoBehaviour
    {
        [System.Serializable]
        public class HealthChangedEvent : UnityEvent<int, int> { }

        [SerializeField] private HealthChangedEvent onHealthChanged;

        public void Bind(PlayerRuntime player)
        {
            if (player == null)
            {
                return;
            }

            player.Health.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(player.Health.CurrentHealth, player.Health.MaxHealth);
        }

        private void HandleHealthChanged(int current, int max)
        {
            onHealthChanged?.Invoke(current, max);
        }
    }
}

