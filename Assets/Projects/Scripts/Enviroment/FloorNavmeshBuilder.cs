using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace Creotly_Studios
{
    public class FloorNavmeshBuilder : MonoBehaviour
    {
        public static FloorNavmeshBuilder Instance;

        //Components
        private Sentient sentient;
        private NavMeshData navMeshData;
        private NavMeshSurface navMeshSurface;

        //Parameters
        private Vector3 worldAnchor;
        private WaitForSeconds waitForSeconds;
        private List<NavMeshModifier> navMeshModifiers = new List<NavMeshModifier>();
        private List<NavMeshBuildSource> navMeshSources = new List<NavMeshBuildSource>();
        private List<NavMeshBuildMarkup> navMeshBuildMarkups = new List<NavMeshBuildMarkup>();

        //Events
        public delegate void NavMeshBuilderEvent(Bounds bounds);
        public NavMeshBuilderEvent OnNavMeshBuilderUpdate;

        [Header("Parameters")]
        [SerializeField] private float updateRate = 0.1f;
        [SerializeField] private float distanceBeforeBake = 3.0f;
        [SerializeField] private Vector3 navMeshAreaBakeSize = new Vector3(55, 55, 55);

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            sentient = GetComponentInParent<Sentient>();
            navMeshSurface = GetComponent<NavMeshSurface>();
            waitForSeconds = new WaitForSeconds(updateRate);
        }

        public void FloorNavMeshBuilder_Start()
        {
            navMeshData = new NavMeshData();
            NavMesh.AddNavMeshData(navMeshData);
            navMeshSurface.navMeshData = navMeshData;

            BuildNavMeshSurface(false);
            StartCoroutine(CheckPlayerMovement());
            OnNavMeshBuilderUpdate += EnemySpawner.Instance.HandleNavMeshUpdate;

            EnemySpawner.Instance.EnemySpawner_Start();
        }

        private void BuildNavMeshSurface(bool asyncBuild)
        {
            AsyncOperation navMeshUpdateOperation = null;
            Bounds navMeshBounds = new Bounds(sentient.playerTransform.position, navMeshAreaBakeSize);

            navMeshModifiers.Clear();
            navMeshBuildMarkups.Clear();
            NavMeshBuilder.CollectSources(navMeshBounds, navMeshSurface.layerMask, navMeshSurface.useGeometry, navMeshSurface.defaultArea, navMeshBuildMarkups, navMeshSources);

            Bounds buildBound = new Bounds(sentient.playerTransform.position, navMeshAreaBakeSize);
            if(asyncBuild)
            {
                navMeshUpdateOperation = NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, navMeshSurface.GetBuildSettings(), navMeshSources, buildBound);
                navMeshUpdateOperation.completed += HandleNavMeshUpdateOperation;
                return;
            }
            NavMeshBuilder.UpdateNavMeshData(navMeshData, navMeshSurface.GetBuildSettings(), navMeshSources, buildBound);
            OnNavMeshBuilderUpdate?.Invoke(buildBound);
        }

        private IEnumerator CheckPlayerMovement()
        {
            while(true)
            {
                float distance = Vector3.Distance(worldAnchor, sentient.playerTransform.position);
                if(distance > distanceBeforeBake)
                {
                    BuildNavMeshSurface(true);
                    worldAnchor = sentient.playerTransform.position;
                }
                yield return waitForSeconds;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Bounds bounds = new Bounds(worldAnchor, 2 * (navMeshAreaBakeSize));
            Gizmos.DrawCube(bounds.center, bounds.extents);
        }

        private void HandleNavMeshUpdateOperation(AsyncOperation asyncOperation)
        {
            Bounds bounds = new Bounds(worldAnchor, navMeshAreaBakeSize);
            OnNavMeshBuilderUpdate?.Invoke(bounds);
        }
    }
}
