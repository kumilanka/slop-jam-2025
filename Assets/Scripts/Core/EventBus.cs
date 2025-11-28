using System;
using UnityEngine;

namespace SlopJam.Core
{
    /// <summary>
    /// Minimal event bus. Keeps runtime events decoupled without over-engineering.
    /// </summary>
    public class EventBus : MonoBehaviour
    {
        public event Action EnemyKilled;
        public event Action<int> PlayerDamaged;
        public event Action PlayerDied;

        public void RaiseEnemyKilled() => EnemyKilled?.Invoke();
        public void RaisePlayerDamaged(int remainingHealth) => PlayerDamaged?.Invoke(remainingHealth);
        public void RaisePlayerDied() => PlayerDied?.Invoke();
    }
}

