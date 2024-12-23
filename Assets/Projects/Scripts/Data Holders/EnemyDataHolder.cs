using UnityEngine;
using UnityEngine.Pool;

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
            aiManagerPool = ObjectPooler.AIManagerPool(enemyObject);
        }
    }
}
