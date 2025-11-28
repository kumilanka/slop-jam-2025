using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 3f;

        private Rigidbody rb;
        private DamageSystem damageSystem;
        private int damage;
        private GameObject instigator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(int damageAmount, float speed, DamageSystem system, GameObject source)
        {
            damage = damageAmount;
            damageSystem = system;
            instigator = source;
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == instigator)
            {
                return;
            }

            if (other.TryGetComponent(out HealthComponent health))
            {
                var request = new DamageRequest(health, damage, instigator, transform.position);
                damageSystem?.ApplyDamage(request);
            }

            Destroy(gameObject);
        }
    }
}

