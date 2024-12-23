using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class EnemySpawner : MonoBehaviour
    {
        private Transform target;
        private Collider[] colliders;

        private const int cnst_tileSize = 20;
        private HashSet<Vector3> spawnedTiles = new HashSet<Vector3>();
        private List<AIManager> spawnedEnemiesList = new List<AIManager>();

        [Header("Component's Parameters")]
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private FloorNavmeshBuilder floorNavmeshBuilder;
        [SerializeField] private SpawnMethod spawnMethod = SpawnMethod.RoundRobin;

        [Header("Spawn Parameters")]
        [SerializeField] private float spawnRadius;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private EnemyDataHolder[] enemyDataHolders;

        [Header("Spawn Statistics")]
        [SerializeField] private int maxEnemies = 5;
        [SerializeField] private float spawnDensityPerTile = 0.5f;
        [SerializeField] private Vector3Int navMeshSize = new Vector3Int(40, 10, 40);
        private void Awake()
        {
            InitializeEnemyDataHolders();
        }
        private void Start()
        {
            colliders = new Collider[maxEnemies];
            floorNavmeshBuilder.OnNavMeshBuilderUpdate += HandleNavMeshUpdate;
        }

        private void InitializeEnemyDataHolders()
        {
            for (int i = 0; i < enemyDataHolders.Length; i++)
            {
                enemyDataHolders[i] = Instantiate(enemyDataHolders[i]);

                EnemyDataHolder enemyDataHolder = enemyDataHolders[i];
                enemyDataHolder.Initialize();
            }
        }

        private void SpawnEnemyOnNewTile(Vector3 currentPosition)
        {
            if(spawnedEnemiesList.Count >= maxEnemies)
            {
                return;
            }

            int navMeshSizeCalc = navMeshSize.x / cnst_tileSize / 2;

            for(int x = -1 * navMeshSizeCalc; x < navMeshSizeCalc; x++)
            {
                for (int z = -1 * navMeshSizeCalc; z < navMeshSizeCalc; z++)
                {
                    int enemiesSpawnedForTile = 0;
                    Vector3 tilePosition = new Vector3(currentPosition.x + x, currentPosition.y, currentPosition.z + z);

                    if(spawnedTiles.Contains(tilePosition) != true)
                    {
                        while (enemiesSpawnedForTile + Random.value < spawnDensityPerTile && spawnedEnemiesList.Count < maxEnemies)
                        {
                            enemiesSpawnedForTile++;
                            HandleSpawnEnemyOnTile(currentPosition);
                        }
                        spawnedTiles.Add(tilePosition);
                    }
                }
            }
        }

        private void HandleSpawnEnemyOnTile(Vector3 currentPosition)
        {
            if(spawnMethod == SpawnMethod.Random)
            {
                Spawn_Random(currentPosition);
                return;
            }
            Spawn_RoundRobin(spawnedEnemiesList.Count, currentPosition);
        }

        private void Spawn_Random(Vector3 currentPosition)
        {
            int random = Random.Range(0, enemyDataHolders.Length);
            SpawnEnemy(random, currentPosition);
        }

        private void Spawn_RoundRobin(int index, Vector3 currentPosition)
        {
            int spawnIndex = index % enemyDataHolders.Length;
            SpawnEnemy(spawnIndex, currentPosition);
        }

        private void SpawnEnemy(int spawnIndex, Vector3 currentPosition)
        {
            EnemyDataHolder enemyData = enemyDataHolders[spawnIndex];

            AIManager aiManager = enemyData.aiManagerPool.Get();

            float random = Random.Range(-cnst_tileSize, cnst_tileSize);
            Vector3 randomPoint = currentPosition * cnst_tileSize + new Vector3(random,0,random);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit navMeshHit, spawnRadius, NavMesh.AllAreas))
            {
                aiManager.transform.SetParent(spawnPoint);
                Vector3 localSpawnPosition = spawnPoint.InverseTransformPoint(navMeshHit.position);
                aiManager.transform.SetPositionAndRotation(localSpawnPosition, Quaternion.identity);

                spawnedEnemiesList.Add(aiManager);
                return;
            }

            spawnedTiles.Remove(currentPosition);
            enemyData.aiManagerPool.Release(aiManager);
            Debug.LogWarning("Could not find suitable navMeshArea");
        }

        private void HandleNavMeshUpdate(Bounds bounds)
        {
            int hits = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, colliders, Quaternion.identity, enemyMask.value);

            AIManager[] aIManagersArray = new AIManager[hits];
            for(int i = 0; i < hits; i++)
            {
                AIManager aiManager = colliders[i].GetComponentInParent<AIManager>();

                if(aiManager != null)
                {
                    aiManager.navMeshAgent.enabled = true;
                }
            }

            HashSet<AIManager> outsideBoundEnemies = new HashSet<AIManager>(spawnedEnemiesList);
            outsideBoundEnemies.ExceptWith(aIManagersArray);

            foreach(AIManager outOfBound in outsideBoundEnemies)
            {
                outOfBound.navMeshAgent.enabled = false;
            }

            if(target == null)
            {
                target = Sentient.Instance.playerTransform;
            }

            Vector3 currentTilePosition = new            
            (
                Mathf.FloorToInt(target.transform.position.x) / cnst_tileSize,
                Mathf.FloorToInt(target.transform.position.y) / cnst_tileSize,
                Mathf.FloorToInt(target.transform.position.z) / cnst_tileSize
            );

            if (spawnedTiles.Contains(currentTilePosition))
            {
                spawnedTiles.Add(currentTilePosition);
            }

            SpawnEnemyOnNewTile(currentTilePosition);
        }
    }
}
