using SlopJam.Player;
using UnityEngine;
using UnityEngine.Events;

namespace SlopJam.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private HUDDocumentView documentView;
        [SerializeField] private UnityEvent onShown;
        [SerializeField] private UnityEvent onHidden;

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerRuntime>();
            if (player != null)
            {
                player.Health.OnDeath += HandlePlayerDeath;
            }
            Hide();
        }

        private void HandlePlayerDeath()
        {
            Debug.Log("Player died - stopping gameplay loop.");
            Time.timeScale = 0f;
            Show();
        }

        public void Show()
        {
            documentView?.ShowGameOver();
            onShown?.Invoke();
        }

        public void Hide()
        {
            documentView?.HideGameOver();
            onHidden?.Invoke();
        }
    }
}

