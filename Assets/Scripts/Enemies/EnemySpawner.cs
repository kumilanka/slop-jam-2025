using System.Collections.Generic;
using SlopJam.Player;
using UnityEngine;

namespace SlopJam.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyBrain enemyPrefab;
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private List<Transform> spawnPoints = new();
        [SerializeField] private int simultaneousEnemies = 3;

        private readonly List<EnemyBrain> activeEnemies = new();
        private PlayerRuntime player;

        public void Initialize(PlayerRuntime playerRuntime)
        {
            player = playerRuntime;
        }

        private void Update()
        {
            if (player == null || enemyPrefab == null || enemyConfig == null)
            {
                return;
            }

            CleanupDeadEnemies();
            while (activeEnemies.Count < simultaneousEnemies)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("EnemySpawner has no spawn points configured.");
                return;
            }

            var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var enemy = Instantiate(enemyPrefab, point.position, point.rotation, transform);
            enemy.SetTarget(player.transform);
            activeEnemies.Add(enemy);
        }

        private void CleanupDeadEnemies()
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i] == null || !activeEnemies[i].gameObject.activeInHierarchy)
                {
                    activeEnemies.RemoveAt(i);
                }
            }
        }
    }
}

