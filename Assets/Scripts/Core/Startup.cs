using SlopJam.Combat;
using SlopJam.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlopJam.Bootstrap
{
    /// <summary>
    /// Attach to the bootstrap scene root. Responsible for registering services and
    /// loading the gameplay scene additively so other systems can subscribe.
    /// </summary>
    public class Startup : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private InputService inputService;
        [SerializeField] private DamageSystem damageSystem;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (inputService != null)
            {
                ServiceLocator.Register(inputService);
            }

            if (damageSystem != null)
            {
                ServiceLocator.Register(damageSystem);
            }

            LoadGameplayScene();
        }

        private void LoadGameplayScene()
        {
            if (string.IsNullOrEmpty(gameplaySceneName))
            {
                Debug.LogWarning("Gameplay scene name is not configured.");
                return;
            }

            if (!SceneManager.GetSceneByName(gameplaySceneName).isLoaded)
            {
                SceneManager.LoadSceneAsync(gameplaySceneName, LoadSceneMode.Additive);
            }
        }
    }
}

