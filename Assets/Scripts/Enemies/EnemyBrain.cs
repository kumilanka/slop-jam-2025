using System.Collections;
using SlopJam.Combat;
using SlopJam.Core;
using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Enemies
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyBrain : MonoBehaviour, IKnockbackable
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private float stoppingDistance = 0.75f;
        [SerializeField] private float knockbackDamping = 5f;

        private Transform target;
        private HealthComponent health;
        private DamageSystem damageSystem;
        private Rigidbody2D rb;
        private bool canAttack = true;
        private Vector3 externalVelocity;

        public void SetTarget(Transform player)
        {
            target = player;
        }

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            health.SetMaxHealth(config != null ? config.maxHealth : 1);
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
            direction.z = 0f; // 2D Game uses XY plane
            
            if (direction.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            var distance = direction.magnitude;
            if (distance <= stoppingDistance && canAttack)
            {
                AttemptAttack();
            }
        }

        private void FixedUpdate()
        {
            if (target == null || config == null || !health.IsAlive)
            {
                return;
            }

            if (externalVelocity.sqrMagnitude > 0.0001f)
            {
                externalVelocity = Vector3.MoveTowards(externalVelocity, Vector3.zero, knockbackDamping * Time.fixedDeltaTime);
            }

            var direction = (target.position - transform.position);
            direction.z = 0f;
            var distance = direction.magnitude;
            Vector3 voluntaryMovement = Vector3.zero;

            if (distance > stoppingDistance)
            {
                voluntaryMovement = direction.normalized * config.moveSpeed;
            }

            rb.linearVelocity = voluntaryMovement + externalVelocity;
        }

        public void ApplyKnockback(Vector3 impulse)
        {
            externalVelocity += impulse;
        }

        private void AttemptAttack()
        {
            if (target.TryGetComponent(out HealthComponent targetHealth))
            {
                var request = new DamageRequest(targetHealth, config.damage, gameObject, target.position);
                _ = damageSystem?.ApplyDamage(request);
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

