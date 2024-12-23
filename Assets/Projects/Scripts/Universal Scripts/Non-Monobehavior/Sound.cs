using System.Collections.Generic;
using UnityEngine;

namespace Creotly_Studios
{
    public class Sound
    {
        [field: SerializeField] public float soundRange {get; private set;}
        [field: SerializeField] public Vector3 soundPosition {get; private set;}
        [field: SerializeField] public Transform soundSource {get; private set;}

        public void SetParameters(Transform source, float range)
        {
            soundRange = range;
            soundSource = source;
            soundPosition = source.position;
        }
    }
 
    public class MakeSounds : MonoBehaviour
    {
        public static MakeSounds instance;

        [Header("Parameters")]
        public LayerMask layerMask;
        public Collider[] colliders;
        public List<EnemyDetectionScript> detectionScriptsList = new();       

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(instance);
            }
            instance = this;
            colliders = new Collider[20];
        }

        private void FillEnemyManagersList(Sound sound)
        {
            Physics.OverlapSphereNonAlloc(sound.soundPosition, sound.soundRange, colliders, layerMask);
            
            detectionScriptsList.Clear();
            foreach(var collider in colliders)
            {
                if(collider == null)
                {
                    continue;
                }
                
                bool hasDetection = collider.TryGetComponent(out EnemyDetectionScript detectionScript);
                if(hasDetection && detectionScriptsList.Contains(detectionScript) != true)
                {
                    detectionScriptsList.Add(detectionScript);
                }
            }
        }

        public void BeAudible(Sound sound)
        {
            FillEnemyManagersList(sound);
            for(int i = 0; i < detectionScriptsList.Count; i++)
            {
                detectionScriptsList[i].SetSoundTarget(sound);
            }
        }
    }
}
