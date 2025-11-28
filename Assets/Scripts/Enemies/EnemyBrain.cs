using System.Collections;
using SlopJam.Combat;
using SlopJam.Core;
using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Enemies
{
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private float stoppingDistance = 0.75f;

        private Transform target;
        private HealthComponent health;
        private DamageSystem damageSystem;
        private bool canAttack = true;

        public void SetTarget(Transform player)
        {
            target = player;
        }

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            health.SetMaxHealth(config != null ? config.maxHealth : 1);
        }

        private void Start()
        {
            ServiceLocator.TryResolve(out damageSystem);
        }

        private void Update()
        {
            if (target == null || config == null || !health.IsAlive)
            {
                return;
            }

            var direction = (target.position - transform.position);
            direction.y = 0f;
            var distance = direction.magnitude;

            if (distance > stoppingDistance)
            {
                var movement = direction.normalized * config.moveSpeed * Time.deltaTime;
                transform.position += movement;
                transform.forward = direction.normalized;
            }
            else if (canAttack)
            {
                AttemptAttack();
            }
        }

        private void AttemptAttack()
        {
            if (target.TryGetComponent(out HealthComponent targetHealth))
            {
                var request = new DamageRequest(targetHealth, config.damage, gameObject, target.position);
                damageSystem?.ApplyDamage(request);
            }

            StartCoroutine(AttackCooldown());
        }

        private IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(config.attackCooldown);
            canAttack = true;
        }
    }
}

