using UnityEngine;
using UnityEngine.Pool;

namespace Creotly_Studios
{
    public static class ObjectPooler
    {
        public static ObjectPool<Tiles> TilesPool(Tiles objectToPool, ObjectPool<Tiles> SP)
        {
            ObjectPool<Tiles> objectPool = new            (
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
            ObjectPool<TrailRenderer> objectPool = new            (
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
            ObjectPool<GameObject> objectPool = new            (
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
            ObjectPool<ParticleSystem> objectPool = new            (
                () => {return GameObject.Instantiate(objectToPool);},
                spawnObject => {spawnObject.gameObject.SetActive(true);},
                spawnObject => {spawnObject.gameObject.SetActive(false);},
                spawnObject => {GameObject.Destroy(spawnObject);},
                false, 50, 100
            );
            return objectPool;
        }

        public static ObjectPool<BulletFX> BulletFXPool(GameObject decalPrefab, ParticleSystem fxPrefab, MonoBehaviour context)
        {
            ObjectPool<BulletFX> pool = null;

            pool = new ObjectPool<BulletFX>
            (
                () => 
                {
                    var fx = new BulletFX(decalPrefab, fxPrefab, context);
                    fx.SetPool(pool);
                    return fx;
                },
                fx => fx.GetObject(),
                fx => fx.Release(),
                null,
                false, 75, 200
            );
            return pool;
        }

        private static void GetTilesFromPool(Tiles spawnObject, ObjectPool<Tiles> SP)
        {
            spawnObject.ResetTiles(SP);
        }
    }
}
