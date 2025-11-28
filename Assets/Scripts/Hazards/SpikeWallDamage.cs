using SlopJam.Combat;
using SlopJam.Core;
using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Hazards
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpikeWallDamage : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private Transform centerOverride;
        [SerializeField] private bool shareCenterFromParent = true;

        private DamageSystem damageSystem;
        private Collider2D hazardCollider;
        private Rigidbody2D hazardBody;

        private void Reset()
        {
            SetupCollider();
            SetupBody();
        }

        private void Awake()
        {
            SetupCollider();
            SetupBody();

            if (shareCenterFromParent && centerOverride == null)
            {
                // centerOverride = transform.parent; 
                // Do NOT set this to transform.parent for SpikeWallLeft/Right because their parent is SpikeWallManager
                // which might be offset from the actual "center" of the arena.
                
                // Instead, let's try to find the "ArenaCenter" explicitly if it exists in parent or scene,
                // or default to Vector3.zero if that's the intent.
                
                // However, for "ClosingSpikeWalls", the parent is the manager.
                // If the manager has a centerOverride, we should probably use that?
                
                // Checking the scene structure provided: 
                // SpikeWallManager -> centerOverride (ArenaCenter)
                // SpikeWallLeft -> parent is SpikeWallManager
                
                // If this script is on SpikeWallLeft, transform.parent is SpikeWallManager.
                // SpikeWallManager's position is -20.02, -2.95. ArenaCenter is -17.58, -2.22.
                
                // If we use transform.parent.position as "center", we push away from -20,-2.
                // If we use ArenaCenter, we push away from -17,-2.
                
                // The left wall is at x ~ -41 (far left). Pushing away from center (-17) means pushing LEFT (further out)?
                // Wait.
                
                // Left Wall Position: x = -41. Center x = -17.
                // Direction = Player(-20) - Center(-17) = -3 (Left).
                // If Player is at -20 (between wall and center), push away from center -> Push Left -> Towards Wall?
                // No, that's wrong.
                
                // We want to push away from the WALL surface.
                // Or push towards the "Safe Zone" (Center).
                
                // Current Logic:
                // direction = (target - center).normalized.
                // If target is at -20, center is -17. Direction is (-3, 0). Left.
                // Knockback pushes Player Left -> Into the left wall.
                
                // We want to push Player towards the center (Right).
                // So direction should be (Center - WallPosition)? No.
                
                // If using OnCollision, we use the normal.
                // Collision normal points OUT of the collider.
                // If Player hits Left Wall (which is to the left), the normal should point Right.
                // So OnCollision logic should be correct (using normal).
                
                // The issue mentioned is "Left wall incorrectly move through it".
                // This suggests isTrigger might be true, or physics layers are wrong.
                
                // In the YAML provided for SpikeWallLeft (244026924), m_IsTrigger: 1.
                // In the YAML provided for SpikeWallRight (970717579), m_IsTrigger: 1.
                
                // My code in SetupCollider forces isTrigger = false.
                // BUT, SetupCollider is called in Reset (Editor time) and Awake (Runtime).
                // If the scene data has isTrigger=1, and Awake sets it to false, it should work at runtime.
                
                // HOWEVER, the scene YAML shows SpikeWallLeft has rotation Z=0.707 (90 degrees?).
                // m_LocalRotation: {x: 0, y: -0, z: 0.7071068, w: -0.7071068} -> Euler (0, 0, 270) or -90.
                
                // If the BoxCollider2D is rotated, its local axes change.
                
                // Left Wall: Rotated 270 degrees.
                // Right Wall: Rotation (0,0,0).
                
                // If SetupBody sets FreezeRotation, does that mess with initial rotation?
                // Rigidbody2D.constraints only affects physics simulation integration, not transform.
                
                // The user says "Left wall ... incorrectly move through it".
                // This strongly implies the collider is a Trigger at runtime for some reason,
                // OR the player is being pushed INTO it instead of away from it, effectively trapping/tunneling.
            }
        }

        private void Start()
        {
            if (!ServiceLocator.TryResolve(out damageSystem))
            {
                damageSystem = FindFirstObjectByType<DamageSystem>();
            }
            
            // Force isTrigger false again just to be sure, though Awake should handle it.
            if (hazardCollider != null) hazardCollider.isTrigger = false;

            // Check if the collider needs to be resized to match the sprite
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var boxCollider = hazardCollider as BoxCollider2D;
            if (spriteRenderer != null && boxCollider != null && spriteRenderer.drawMode != SpriteDrawMode.Simple)
            {
                // For Sliced or Tiled sprites, the Collider might not match the visual size if not updated.
                // The SpriteRenderer.size gives the local size of the sliced sprite.
                // We should update the BoxCollider to match this size.
                if (boxCollider.size != spriteRenderer.size)
                {
                    Debug.Log($"[SpikeWallDamage] Resizing collider on {name} from {boxCollider.size} to match sprite size {spriteRenderer.size}");
                    boxCollider.size = spriteRenderer.size;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // If we are solid, this shouldn't happen for the main body, but keeping it for safety
            TryDamage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // When hitting a solid wall, use the contact normal.
            // The normal points from the wall towards the player.
            // This is exactly the direction we want to push the player (away from wall).
            var contact = collision.GetContact(0);
            TryDamage(collision.collider, contact.normal, contact.point);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
             var contact = collision.GetContact(0);
             TryDamage(collision.collider, contact.normal, contact.point);
        }

        private void TryDamage(Collider2D other, Vector3? overrideDirection = null, Vector3? overrideHitPoint = null)
        {
            // Check on object or parent for HealthComponent
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health == null)
            {
                health = other.GetComponentInParent<HealthComponent>();
            }

            if (health == null)
            {
                return;
            }

            // Calculate hit point: Prefer the collision contact point if available, otherwise use ClosestPoint.
            Vector3 hitPoint = overrideHitPoint.HasValue 
                ? overrideHitPoint.Value 
                : other.ClosestPoint(transform.position);

            var request = new DamageRequest(health, damage, gameObject, hitPoint);
            var applied = damageSystem != null
                ? damageSystem.ApplyDamage(request)
                : health.ApplyDamage(request);

            if (!applied)
            {
                return;
            }

            // Check on object or parent for IKnockbackable
            IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
            if (knockbackable == null)
            {
                knockbackable = other.GetComponentInParent<IKnockbackable>();
            }

            if (knockbackable != null)
            {
                ApplyKnockback(health.transform, knockbackable, overrideDirection);
            }
        }

        private void ApplyKnockback(Transform targetTransform, IKnockbackable target, Vector3? overrideDirection)
        {
            Vector3 direction;
            
            if (overrideDirection.HasValue)
            {
                direction = overrideDirection.Value;
            }
            else
            {
                var center = centerOverride != null ? centerOverride.position : transform.position;
                // Direction from center TO target (push away)
                direction = (targetTransform.position - center).normalized;
            }

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = Vector3.up;
            }

            target.ApplyKnockback(direction * knockbackForce);
        }

        private void SetupCollider()
        {
            hazardCollider = GetComponent<Collider2D>();
            if (hazardCollider == null)
            {
                hazardCollider = gameObject.AddComponent<BoxCollider2D>();
            }
            // Ensure it is solid, not a trigger, to prevent walking through
            hazardCollider.isTrigger = false;
        }

        private void SetupBody()
        {
            hazardBody = GetComponent<Rigidbody2D>();
            if (hazardBody == null)
            {
                hazardBody = gameObject.AddComponent<Rigidbody2D>();
            }
            hazardBody.bodyType = RigidbodyType2D.Kinematic;
            hazardBody.gravityScale = 0f;
            hazardBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            hazardBody.useFullKinematicContacts = true;
        }
    }
}

