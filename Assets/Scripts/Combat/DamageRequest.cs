using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Combat
{
    public readonly struct DamageRequest
    {
        public DamageRequest(HealthComponent target, int amount, GameObject instigator, Vector3 hitPoint)
        {
            Target = target;
            Amount = amount;
            Instigator = instigator;
            HitPoint = hitPoint;
        }

        public HealthComponent Target { get; }
        public int Amount { get; }
        public GameObject Instigator { get; }
        public Vector3 HitPoint { get; }
    }
}

