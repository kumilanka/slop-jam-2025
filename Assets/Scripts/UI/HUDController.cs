using SlopJam.Player;
using UnityEngine;

namespace SlopJam.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private HUDDocumentView documentView;

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
            documentView?.UpdateHealth(current, max);
        }
    }
}

