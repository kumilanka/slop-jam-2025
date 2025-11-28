using SlopJam.Combat;
using SlopJam.Core;
using UnityEngine;

namespace SlopJam.Player
{
    [RequireComponent(typeof(HealthComponent))]
    public class PlayerRuntime : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private WeaponController weaponController;

        public PlayerConfig Config => config;
        public WeaponController Weapon => weaponController;
        public HealthComponent Health { get; private set; }

        private void Awake()
        {
            Health = GetComponent<HealthComponent>();
            if (config != null)
            {
                Health.SetMaxHealth(config.maxHealth);
            }
        }

        private void Start()
        {
            if (weaponController != null && config != null && config.startingWeapon != null)
            {
                weaponController.Configure(config.startingWeapon);
            }
        }
    }
}

