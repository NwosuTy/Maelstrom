using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Enemy Data Holder", menuName = "Creotly/EnemyDataHolder")]
    public class EnemyDataHolder : ScriptableObject
    {
        public ObjectPool<AIManager> aiManagerPool;

        [Header("Pool Object")]
        [SerializeField] private AIManager enemyObject;

        public void Initialize()
        {
            aiManagerPool = new ObjectPool<AIManager>
            (
                () => { return Instantiate(enemyObject); },
                spawnObject => { spawnObject.gameObject.SetActive(false); },
                spawnObject => { spawnObject.gameObject.SetActive(false); },
                spawnObject => { Destroy(spawnObject); },
                false, 500, 700
            );
        }

        public IEnumerator GetObject(AIManager aiManager, Vector3 position)
        { 
            aiManager.dataHolder = this;
            aiManager.transform.SetPositionAndRotation(position, Quaternion.identity);

            yield return new WaitUntil(() => aiManager.transform.position == position);

            aiManager.canUpdate = true;
            aiManager.gameObject.SetActive(true);
        }
    }
}
