using UnityEngine;
using UnityEngine.Pool;

namespace Creotly_Studios
{
    public static class ObjectPooler
    {
        public static ObjectPool<Tiles> TilesPool(Tiles objectToPool, ObjectPool<Tiles> SP)
        {
            ObjectPool<Tiles> objectPool = new ObjectPool<Tiles>
            (
                () => {return GameObject.Instantiate(objectToPool);},
                spawnObject => {GetTilesFromPool(spawnObject, SP);},
                spawnObject => {spawnObject.gameObject.SetActive(false);},
                spawnObject => {GameObject.Destroy(spawnObject.gameObject);},
                false, 400, 500
            );
            return objectPool;
        }

        public static ObjectPool<TrailRenderer> TrailPool(TrailRenderer objectToPool)
        {
            ObjectPool<TrailRenderer> objectPool = new ObjectPool<TrailRenderer>
            (
                () => { return GameObject.Instantiate(objectToPool); },
                spawnObject => { spawnObject.gameObject.SetActive(true); },
                spawnObject => { spawnObject.gameObject.SetActive(false); },
                spawnObject => { GameObject.Destroy(spawnObject.gameObject); },
                false, 400, 500
            );
            return objectPool;
        }

        public static ObjectPool<GameObject> GameObjectPool(GameObject objectToPool)
        {
            ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>
            (
                () => {return GameObject.Instantiate(objectToPool);},
                spawnObject => {spawnObject.SetActive(true);},
                spawnObject => {spawnObject.SetActive(false);},
                spawnObject => {GameObject.Destroy(spawnObject);},
                false, 50, 100
            );
            return objectPool;
        }

        public static ObjectPool<ParticleSystem> ParticlePool(ParticleSystem objectToPool)
        {
            ObjectPool<ParticleSystem> objectPool = new ObjectPool<ParticleSystem>
            (
                () => {return GameObject.Instantiate(objectToPool);},
                spawnObject => {spawnObject.gameObject.SetActive(true);},
                spawnObject => {spawnObject.gameObject.SetActive(false);},
                spawnObject => {GameObject.Destroy(spawnObject);},
                false, 50, 100
            );
            return objectPool;
        }

        private static void GetTilesFromPool(Tiles spawnObject, ObjectPool<Tiles> SP)
        {
            spawnObject.ResetTiles(SP);
        }
    }
}
