using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class RandomBuilding : MonoBehaviour
    {
        private Vector3 currentScale;

        [Header("Parameters")]
        public bool hasBeenSet;
        [SerializeField] private List<Transform> spawnPoints = new();

        private void Awake()
        {
            GetSpawnPoints();
        }

        public void EnableRandomBuildings(float delta)
        {
            if(hasBeenSet == true)
            {
                return;
            }

            foreach(Transform spawnPoint in spawnPoints)
            {
                if(spawnPoint.childCount <= 0)
                {
                    continue;
                }

                int random = Random.Range(0, spawnPoint.childCount);
                Transform activeObject = spawnPoint.GetChild(random);

                currentScale = activeObject.localScale;
                activeObject.localScale = Vector3.zero;

                activeObject.gameObject.SetActive(true);
                activeObject.DOScale(currentScale, 1 * delta).SetEase(Ease.OutElastic);
            }
            hasBeenSet = true;
        }

        private void GetSpawnPoints()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform directChild = transform.GetChild(i);
                if(directChild.parent != transform)
                {
                    continue;
                }
                spawnPoints.Add(directChild);
            }
        }
    }
}
