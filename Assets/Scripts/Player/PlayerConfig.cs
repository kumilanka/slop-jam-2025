using UnityEngine;

namespace SlopJam.Player
{
    [CreateAssetMenu(menuName = "SlopJam/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        public float moveSpeed = 5f;
        public int maxHealth = 5;
        public float fireCooldown = 0.25f;
        public Combat.WeaponDefinition startingWeapon;
    }
}

