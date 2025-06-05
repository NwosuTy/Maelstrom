using UnityEngine;
using UnityEngine.Pool;
using Unity.Cinemachine;
using System.Collections;

namespace Creotly_Studios
{
    [System.Serializable]
    public class HumanBones
    {
        public HumanBodyBones bone;
        public Transform boneTransform;
        [Range(0f ,1f)] public float weight;
    }

    public class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;

        public Bullet(Vector3 pos, Vector3 vel)
        {
            time = 0.0f;
            initialPosition = pos;
            initialVelocity = vel;
        }
    }

    [System.Serializable]
    public class GunRecoil
    {
        private int index;
        public float duration;
        public Vector2[] recoilPatterns;

        private Transform cameraObject;
        private CinemachineImpulseSource impulseSource;

        public void Initialize(WeaponManager origin, Transform cameraObj)
        {
            cameraObject = cameraObj;
            impulseSource = origin.GetComponent<CinemachineImpulseSource>();
        }

        private int NextIndex()
        {
            return (index++) % recoilPatterns.Length;
        }

        public void GenerateRecoil(float delta)
        {
            if(impulseSource == null)
            {
                return;
            }

            float vertical = recoilPatterns[index].y;
            float horizontal = recoilPatterns[index].x;

            float yAxis = ((vertical / 1000) * delta) / duration;
            float xAxis = ((horizontal / 25) * delta) / duration;

            index = NextIndex();
            Vector2 force = new(xAxis, yAxis);
            impulseSource.GenerateImpulse(force);
        }
    }

    [System.Serializable]
    public class BulletFX
    {
        private ObjectPool<BulletFX> pool;
        private ObjectPool<GameObject> bulletDecalPool;

        private MonoBehaviour context;
        private ParticleSystem bulletImpactFX;

        private WaitForSeconds impactDelay = new(2f);
        private WaitForSeconds decalDelay = new(7.5f);

        public BulletFX(GameObject decalPrefab, ParticleSystem impactFXPrefab, MonoBehaviour context)
        {
            bulletImpactFX = GameObject.Instantiate(impactFXPrefab);
            bulletDecalPool = ObjectPooler.GameObjectPool(decalPrefab);
            this.context = context;
        }

        public void SetPool(ObjectPool<BulletFX> pool) => this.pool = pool;

        public void GetObject()
        {
            bulletImpactFX.gameObject.SetActive(true);
        }

        public void HandleBulletImpact(Vector3 pos, Quaternion rot)
        {
            bulletImpactFX.transform.SetPositionAndRotation(pos, rot);
            bulletImpactFX.Emit(1);

            var decal = bulletDecalPool.Get();
            decal.transform.SetPositionAndRotation(pos, rot);
            decal.transform.Rotate(Vector3.forward, Random.Range(0, 360));
            decal.SetActive(true);

            context.StartCoroutine(ReleaseDecalAfterDelay(decal));
            pool.Release(this);
        }

        public void Release()
        {
            context.StartCoroutine(DisableImpactFX());
        }

        private IEnumerator DisableImpactFX()
        {
            yield return impactDelay;
            bulletImpactFX.gameObject.SetActive(false);
        }

        private IEnumerator ReleaseDecalAfterDelay(GameObject decal)
        {
            yield return decalDelay;
            bulletDecalPool.Release(decal);
        }
    }

    public static class TrailFX
    {
        public static void HandleTrailFX(float simulationSpeed, Vector3 start, Vector3 end, ObjectPool<TrailRenderer> trailPool, MonoBehaviour mb)
        {
            mb.StartCoroutine(TrailFXRoutine(simulationSpeed, start, end, trailPool));
        }

        private static IEnumerator TrailFXRoutine(float simulationSpeed, Vector3 start, Vector3 end, ObjectPool<TrailRenderer> trailPool)
        {
            TrailRenderer trail = trailPool.Get();
            Transform trailTransform = trail.transform;

            trailTransform.position = start;
            yield return null;

            trail.emitting = true;
            float distance = Vector3.Distance(start, end);
            float remainingDistance = distance;
            while (remainingDistance > 0f)
            {
                trailTransform.position = Vector3.Lerp(start, end, Mathf.Clamp01(1 - (remainingDistance / distance)));
                remainingDistance -= simulationSpeed * Time.deltaTime;
                yield return null;
            }

            trailTransform.position = end;
            yield return new WaitForSeconds(trail.time);
            yield return null;

            trail.emitting = false;
            trailTransform.position = end;
            trailPool.Release(trail);
        }
    }
}
