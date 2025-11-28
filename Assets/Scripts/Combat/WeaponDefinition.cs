using UnityEngine;

namespace SlopJam.Combat
{
    [CreateAssetMenu(menuName = "SlopJam/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        public float fireCooldown = 0.2f;
        public int damage = 1;
        public float projectileSpeed = 15f;
        public Projectile projectilePrefab;
    }
}

