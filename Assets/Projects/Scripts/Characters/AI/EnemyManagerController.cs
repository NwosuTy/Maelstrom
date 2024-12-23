using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class EnemyManagerController : MonoBehaviour
    {
        private Sentient sentient;
        private int numberOfSpawns;
        private ObjectPool<AIManager> aiManagersPool;

        [Header("Parameters")]
        public int spawnQuantity;
        [SerializeField] private float spawnRadius;
        [SerializeField] private bool completedSpawn;
        [SerializeField] private BoundaryInt spawnCountRange;

        [Header("Spawn Parameters")]
        [SerializeField] private UIBar aiHealthBarUI;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Canvas healthBarsCanvas;
        [SerializeField] private AIManager[] aiManagersToSpawn;

        [Header("Spawned Objects")]
        public List<UIBar> spawnedHealthBars;
        public List<AIManager> spawnedEnemies;

        private void Awake()
        {
            sentient = GetComponentInParent<Sentient>();
            
            int random = Random.Range(0, aiManagersToSpawn.Length);
            aiManagersPool = ObjectPooler.AIManagerPool(aiManagersToSpawn[random]);
        }

        private int GetSpawnQuantity()
        {
            return Random.Range(spawnCountRange.lowerBound, spawnCountRange.upperBound);
        }

        public void HandleSpawnEnemies()
        {
            if(completedSpawn)
            {
                return;
            }

            spawnQuantity = GetSpawnQuantity();

            while(numberOfSpawns <= spawnQuantity)
            {
                SpawnEnemy();
            }
            completedSpawn = true;
        }

        private void SpawnEnemy()
        {
            AIManager spawnedAI = aiManagersPool.Get();

            spawnedAI.transform.SetParent(spawnPoint);
            Vector3 spawnedPosition = Maths_PhysicsHelper.SpawnPoint(spawnRadius, sentient.playerTransform);

            Vector3 aiPosition = spawnPoint.InverseTransformPoint(spawnedPosition);
            spawnedAI.transform.SetLocalPositionAndRotation(aiPosition, Quaternion.identity);
            
            spawnedEnemies.Add(spawnedAI);
            numberOfSpawns++;
        }
    }
}
