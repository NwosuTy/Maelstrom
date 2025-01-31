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
}
