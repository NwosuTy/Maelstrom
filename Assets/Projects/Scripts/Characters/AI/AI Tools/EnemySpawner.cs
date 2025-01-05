using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;

        private Sentient sentient;
        private int spawnedEnemies;
        private const int cnst_tileSize = 10;

        private Vector3 randomPosition;
        private Collider[] enemyColliders;

        private HashSet<Vector3> spawnedTiles = new HashSet<Vector3>();
        private List<AIManager> spawnedEnemiesList = new List<AIManager>();

        [Header("Parameters")]
        [SerializeField] private int maxEnemies;
        [SerializeField] private Transform parent;
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("Spawn Parameters")]
        [SerializeField] private float spawnRadius;
        [SerializeField] private float spawnDensityPerTile = 0.5f;
        [SerializeField] private EnemyDataHolder[] enemyDataHolders;
        [SerializeField] private SpawnMethod spawnMethod = SpawnMethod.RoundRobin;

        [Header("NavMesh Parameters")]
        [SerializeField] private Vector3Int navMeshSize = new Vector3Int(80, 0, 80);

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            Initialize();
        }

        public void EnemySpawner_Start()
        {
            enemyColliders = new Collider[maxEnemies];
        }

        private void Initialize()
        {
            for (int i = 0; i < enemyDataHolders.Length; i++)
            {
                enemyDataHolders[i] = Instantiate(enemyDataHolders[i]);
                enemyDataHolders[i].Initialize();
            }

            sentient = GetComponentInParent<Sentient>();
        }

        private void SpawnEnemiesOnNewTile(Vector3 currentTilePosition)
        {
            if(spawnedEnemies >= maxEnemies)
            {
                return;
            }

            int navMeshCalc = navMeshSize.x / cnst_tileSize / 2;
            for (int x = -1 * navMeshCalc; x < navMeshCalc; x++)
            {
                for (int z = -1 * navMeshCalc; z < navMeshCalc; z++)
                {
                    int enemiesSpawnedPerTile = 0;
                    Vector3 tilePosition = new Vector3(currentTilePosition.x + x, currentTilePosition.y, currentTilePosition.z + z);

                    if (spawnedTiles.Contains(tilePosition) != true)
                    {
                        while (enemiesSpawnedPerTile + Random.value < spawnDensityPerTile && spawnedEnemies < maxEnemies)
                        {
                            SpawnEnemyOnTile(tilePosition);
                           
                            enemiesSpawnedPerTile++;
                            spawnedEnemies++;
                        }
                    }
                }
            }
        }

        private void SpawnEnemyOnTile(Vector3 tilePosition)
        {
            if(spawnMethod == SpawnMethod.Random)
            {
                SpawnEnemy_Random(tilePosition);
                return;
            }
            SpawnEnemy_RoundRobin(spawnedEnemies, tilePosition);
        }

        private void SpawnEnemy_Random(Vector3 tilePosition)
        {
            int random = Random.Range(0, enemyDataHolders.Length);
            HandleSpawnEnemy(random, tilePosition);
        }

        private void SpawnEnemy_RoundRobin(int index, Vector2 tilePosition)
        {
            int spawnIndex = index % enemyDataHolders.Length;
            HandleSpawnEnemy(spawnIndex, tilePosition);
        }

        private void HandleSpawnEnemy(int index, Vector3 tilePosition)
        {
            randomPosition = tilePosition * cnst_tileSize + new 
                Vector3(Random.Range(-27.5f, 27.5f), 0.0f, Random.Range(-27.5f, 27.5f));

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit navMeshHit, spawnRadius, NavMesh.AllAreas))
            {
                EnemyDataHolder dataHolder = enemyDataHolders[index];
                AIManager aiManager = dataHolder.aiManagerPool.Get();

                aiManager.transform.SetParent(parent);
                Vector3 localPosition = parent.InverseTransformPoint(navMeshHit.position);

                StartCoroutine(dataHolder.GetObject(aiManager, localPosition));
                spawnedEnemiesList.Add(aiManager);
                return;
            }

            spawnedEnemies--;
            spawnedTiles.Remove(tilePosition);
            Debug.LogWarning($"The Position {randomPosition} is not on a suitable NavMesh Area");
        }

        public void HandleNavMeshUpdate(Bounds bounds)
        {
            int hits = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, enemyColliders, Quaternion.identity);

            AIManager[] enemyArrays = new AIManager[hits];
            for (int i = 0; i < hits; i++)
            {
                AIManager aiManager = enemyColliders[i].GetComponentInParent<AIManager>();
                if(aiManager == null)
                {
                    continue;
                }

                enemyArrays[i] = aiManager;
                aiManager.navMeshAgent.enabled = true;
            }

            HashSet<AIManager> outOfBoundsEnemy = new HashSet<AIManager>();
            outOfBoundsEnemy.ExceptWith(enemyArrays);

            foreach(AIManager ai in  outOfBoundsEnemy)
            {
                ai.navMeshAgent.enabled = false;
            }

            Transform playerTransform = sentient.playerTransform;
            Vector3 currentTilePosition = new Vector3
            (
                Mathf.FloorToInt(playerTransform.position.x) / cnst_tileSize,
                Mathf.FloorToInt(playerTransform.position.y) / cnst_tileSize,
                Mathf.FloorToInt(playerTransform.position.z) / cnst_tileSize
            );

            if (spawnedTiles.Contains(currentTilePosition) != true)
            {
                spawnedTiles.Add(currentTilePosition);
            }
            SpawnEnemiesOnNewTile(currentTilePosition);
        }
    }
}
