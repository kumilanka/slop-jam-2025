using System.Collections;
using SlopJam.Core;
using UnityEngine;

namespace SlopJam.Combat
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;

        private WeaponDefinition definition;
        private bool canShoot = true;
        private DamageSystem damageSystem;

        private void Start()
        {
            ServiceLocator.TryResolve(out damageSystem);
        }

        public void Configure(WeaponDefinition newDefinition)
        {
            definition = newDefinition;
        }

        public void TryShoot(Vector3 direction)
        {
            if (!canShoot || definition == null || definition.projectilePrefab == null)
            {
                if (definition == null)
                {
                    Debug.LogWarning($"{name} weapon has no definition assigned.");
                }
                else if (definition.projectilePrefab == null)
                {
                    Debug.LogWarning($"{name} weapon definition has no projectile prefab.");
                }
                return;
            }

            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            StartCoroutine(FireRoutine(direction.normalized));
        }

        private IEnumerator FireRoutine(Vector3 direction)
        {
            canShoot = false;

            var spawnPoint = muzzle != null ? muzzle.position : transform.position;
            var rotation = Quaternion.LookRotation(Vector3.forward, direction);

            var projectile = Instantiate(definition.projectilePrefab, spawnPoint, rotation);
            projectile.Initialize(definition.damage, definition.projectileSpeed, damageSystem, gameObject);

            yield return new WaitForSeconds(definition.fireCooldown);
            canShoot = true;
        }
    }
}

