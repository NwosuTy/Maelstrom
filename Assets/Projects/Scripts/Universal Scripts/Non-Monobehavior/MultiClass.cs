using UnityEngine;

namespace Creotly_Studios
{
    [System.Serializable]
    public class HumanBones
    {
        public HumanBodyBones bone;
        public Transform boneTransform;
        [Range(0f ,1f)] public float weight;
    }
}
