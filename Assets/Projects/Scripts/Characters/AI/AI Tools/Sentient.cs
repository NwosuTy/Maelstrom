using UnityEngine;
using System.Collections;

namespace Creotly_Studios
{
    public class Sentient : MonoBehaviour
    {
        public static Sentient Instance;

        //Private Parameters
        private UITimeStamp uITimeStamp;
        private WaitForSeconds waitForSeconds;
        private EnvironmentSetter environmentSetter;
        private FloorNavmeshBuilder floorNavmeshBuilder;
        private EnemyManagerController aIManagersController;

        //Target
        public Transform playerTransform {get; private set;}
        public PlayerManager playerManager {get; private set;}

        [Header("Parameters")]
        [SerializeField] private float checkInterval = 1f;

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
            floorNavmeshBuilder = GetComponentInChildren<FloorNavmeshBuilder>();
            aIManagersController = GetComponentInChildren<EnemyManagerController>();

            environmentSetter.InitializeGrid(this);
            waitForSeconds = new WaitForSeconds(checkInterval);
        }

        private void Start()
        {
            playerManager = GameManager.playerManager;
            playerTransform = playerManager.transform;
            uITimeStamp = playerManager.GetComponentInChildren<UITimeStamp>();

            Cells cellToCollapse = FirstCell();
            environmentSetter.EnvironmentGeneration(cellToCollapse);
        }
        private Cells FirstCell()
        {
            Cells cellToCollapse = environmentSetter.cellsList[0];
            cellToCollapse.temporaryTilesList.RemoveAll(x => x.hasBuildings == true);
            return cellToCollapse;
        }
    }
}
