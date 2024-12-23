using UnityEngine;
using UnityEngine.Pool;

namespace Creotly_Studios
{
    public class Tiles : MonoBehaviour
    {
        //Components
        private RandomProps randomProps;
        private RandomBuilding randomBuilding;

        [Header("Source")]
        public Cells cellHolder;
        private ObjectPool<Tiles> sourcePool;

        [Header("Status")]
        public bool hasBuildings;
        public bool canEnableObjects;

        [field: Header("Potential Neighbors")]
        [field: SerializeField] public float topBoundary {get; private set;}
        [field: SerializeField] public float leftBoundary {get; private set;}
        [field: SerializeField] public float rightBoundary {get; private set;}
        [field: SerializeField] public float bottomBoundary {get; private set;}

        private void OnEnable()
        {
            if(canEnableObjects == false)
            {
                return;
            }

            float delta = Time.deltaTime;
            randomProps.EnableRandomProp(delta);
            randomBuilding.EnableRandomBuildings(delta);
            canEnableObjects = false;
        }

        public void ReleaseFromPool()
        {
            sourcePool.Release(this);
        }

        public void ResetTiles(ObjectPool<Tiles> SP)
        {
            if(randomProps == null || randomBuilding == null)
            {
                randomProps = GetComponentInChildren<RandomProps>();
                randomBuilding = GetComponentInChildren<RandomBuilding>();
            }

            sourcePool = SP;
            randomProps.hasBeenSet = false;
            randomBuilding.hasBeenSet = false;
        }
    }
}
