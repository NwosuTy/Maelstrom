using UnityEngine;
using UnityEngine.Pool;

namespace Creotly_Studios
{
    public class GameObjectManager : MonoBehaviour
    {
        //Pools
        public static ObjectPool<GameObject> bloodPrefabPool {get; private set;}

        public static ObjectPool<GameObject> woodBulletHolesPool {get; private set;}
        public static ObjectPool<GameObject> metalBulletHolesPool {get; private set;}
        public static ObjectPool<GameObject> cementBulletHolesPool {get; private set;}

        [Header("Impact Objects")]
        [SerializeField] private GameObject woodBulletHoles;
        [SerializeField] private GameObject metalBulletHoles;
        [SerializeField] private GameObject cementBulletHoles;

        [Header("Objects To Spawn")]
        [SerializeField] private GameObject bloodPrefab;

        private void Awake()
        {
            bloodPrefabPool = ObjectPooler.GameObjectPool(bloodPrefab);

            //Impact Objects
            woodBulletHolesPool = ObjectPooler.GameObjectPool(woodBulletHoles);
            metalBulletHolesPool = ObjectPooler.GameObjectPool(metalBulletHoles);
            cementBulletHolesPool = ObjectPooler.GameObjectPool(cementBulletHoles);
        }
    }
}
