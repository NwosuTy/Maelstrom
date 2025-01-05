using UnityEngine;
using System.Collections;

namespace Creotly_Studios
{
    public class Sentient : MonoBehaviour
    {
        public static Sentient Instance;

        //Private Parameters
        private EnvironmentSetter environmentSetter;

        //Target
        public Transform playerTransform {get; private set;}
        public PlayerManager playerManager {get; private set;}

        [Header("Parameters")]
        [SerializeField] private Transform playerHolder;
        [SerializeField] private PlayerManager playerPrefab;

        [Header("NavMesh")]
        public bool surroundingPlayerCellsCollapsed;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            environmentSetter = GetComponentInChildren<EnvironmentSetter>();
            environmentSetter.InitializeGrid(this);
        }

        private void Start()
        {
            Cells cellToCollapse = FirstCell();
            StartCoroutine(SpawnPlayer(cellToCollapse));
            environmentSetter.EnvironmentGeneration(cellToCollapse);
        }

        public IEnumerator SpawnPlayer(Cells cells)
        {
            yield return new WaitUntil(() => cells.hasCollapsed == true);

            playerManager = Instantiate(playerPrefab, playerHolder);
            playerTransform = playerManager.transform;

            GameManager.Instance.SetPlayerManager(playerManager);

            yield return new WaitUntil(() => playerManager.isGrounded == true);

            FloorNavmeshBuilder.Instance.FloorNavMeshBuilder_Start();
        }

        private Cells FirstCell()
        {
            Cells cellToCollapse = environmentSetter.cellsList[0];
            cellToCollapse.temporaryTilesList.RemoveAll(x => x.hasBuildings == true);
            return cellToCollapse;
        }
    }
}
