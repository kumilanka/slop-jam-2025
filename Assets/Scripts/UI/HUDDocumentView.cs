using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlopJam.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HUDDocumentView : MonoBehaviour
    {
        [SerializeField] private string healthValueElement = "health-value";
        [SerializeField] private string heartsElement = "hearts";
        [SerializeField] private string gameOverPanelElement = "game-over-panel";
        [SerializeField] private string retryButtonElement = "retry-button";
        [SerializeField] private string quitButtonElement = "quit-button";

        public event Action RetryRequested;
        public event Action QuitRequested;

        private UIDocument document;
        private Label healthLabel;
        private VisualElement heartsContainer;
        private VisualElement gameOverPanel;
        private Button retryButton;
        private Button quitButton;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            healthLabel = root.Q<Label>(healthValueElement);
            heartsContainer = root.Q<VisualElement>(heartsElement);
            gameOverPanel = root.Q<VisualElement>(gameOverPanelElement);
            retryButton = root.Q<Button>(retryButtonElement);
            quitButton = root.Q<Button>(quitButtonElement);

            if (retryButton != null)
            {
                retryButton.clicked += () => RetryRequested?.Invoke();
            }

            if (quitButton != null)
            {
                quitButton.clicked += () => QuitRequested?.Invoke();
            }

            HideGameOver();
        }

        public void UpdateHealth(int current, int max)
        {
            if (healthLabel != null)
            {
                healthLabel.text = $"{current} / {max}";
            }

            UpdateHearts(current, max);
        }

        public void ShowGameOver()
        {
            gameOverPanel?.RemoveFromClassList("hidden");
        }

        public void HideGameOver()
        {
            gameOverPanel?.AddToClassList("hidden");
        }

        private void UpdateHearts(int current, int max)
        {
            if (heartsContainer == null)
            {
                return;
            }

            while (heartsContainer.childCount < max)
            {
                var heart = new VisualElement();
                heart.AddToClassList("heart");
                heartsContainer.Add(heart);
            }

            for (int i = 0; i < heartsContainer.childCount; i++)
            {
                var heart = heartsContainer[i];
                var isActive = i < current;
                heart.EnableInClassList("heart--active", isActive);
            }
        }
    }
}

