using UnityEngine;
using UnityEngine.Events;

namespace SlopJam.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private HUDDocumentView documentView;
        [SerializeField] private UnityEvent onShown;
        [SerializeField] private UnityEvent onHidden;

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

