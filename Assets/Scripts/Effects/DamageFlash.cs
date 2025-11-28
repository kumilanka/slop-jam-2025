using System.Collections;
using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(HealthComponent))]
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField] private float flashDuration = 0.1f;

        private SpriteRenderer spriteRenderer;
        private HealthComponent health;
        private Color originalColor;
        private Coroutine flashRoutine;
        private int lastHealth;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            health = GetComponent<HealthComponent>();
            originalColor = spriteRenderer.color;
        }

        private void Start()
        {
            lastHealth = health.CurrentHealth;
            health.OnHealthChanged += HandleHealthChanged;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void HandleHealthChanged(int current, int max)
        {
            if (current < lastHealth)
            {
                Flash();
            }
            lastHealth = current;
        }

        private void Flash()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                spriteRenderer.color = originalColor; // Reset before starting new
            }
            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            flashRoutine = null;
        }
    }
}

