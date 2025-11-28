using UnityEngine;

namespace SlopJam.Enemies
{
    [CreateAssetMenu(menuName = "SlopJam/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        public float moveSpeed = 3f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1f;
        public int damage = 1;
        public int maxHealth = 2;
    }
}

