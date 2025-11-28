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
                return;
            }

            StartCoroutine(FireRoutine(direction.normalized));
        }

        private IEnumerator FireRoutine(Vector3 direction)
        {
            canShoot = false;

            var projectile = Instantiate(definition.projectilePrefab, muzzle.position, Quaternion.LookRotation(direction));
            projectile.Initialize(definition.damage, definition.projectileSpeed, damageSystem, gameObject);

            yield return new WaitForSeconds(definition.fireCooldown);
            canShoot = true;
        }
    }
}

