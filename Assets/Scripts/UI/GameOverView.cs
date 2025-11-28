using UnityEngine;
using UnityEngine.Events;

namespace SlopJam.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private UnityEvent onShown;
        [SerializeField] private UnityEvent onHidden;

        private void Awake()
        {
            Hide();
        }

        public void Show()
        {
            if (root != null)
            {
                root.SetActive(true);
            }

            onShown?.Invoke();
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }

            onHidden?.Invoke();
        }
    }
}

