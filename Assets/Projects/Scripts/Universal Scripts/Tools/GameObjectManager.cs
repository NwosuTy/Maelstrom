using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class GameObjectManager : MonoBehaviour
    {
        public static GameObjectManager Instance { get; private set; }

        [Header("Impact Objects")]
        [SerializeField] private ImpactFXConfig[] impactFXConfigs;
        private Dictionary<string, ImpactSurface> tagToSurfaceMap = new();
        private Dictionary<ImpactSurface, ObjectPool<BulletFX>> surfacePoolMap = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Destroy the duplicate
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist between scenes
        }

        private void Start()
        {
            foreach (var config in impactFXConfigs)
            {
                var pool = ObjectPooler.BulletFXPool(config.decal, config.particle, this);
                surfacePoolMap[config.impactSurface] = pool;
                tagToSurfaceMap[config.impactSurface.ToString()] = config.impactSurface;
            }
        }


        public BulletFX GetBulletFX(string tag)
        {
            if (tagToSurfaceMap.TryGetValue(tag, out var surface) && surfacePoolMap.TryGetValue(surface, out var pool))
            {
                return pool.Get();
            }
            Debug.LogWarning($"Unrecognized tag '{tag}'. Using fallback.");
            return surfacePoolMap[ImpactSurface.Stone].Get();
        }

    }
}
