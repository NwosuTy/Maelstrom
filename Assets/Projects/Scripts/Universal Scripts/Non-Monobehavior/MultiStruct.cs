using UnityEngine;

namespace Creotly_Studios
{
    public struct CreotlyTransforms
    {
        public Vector3 pos {  get; private set; }
        public Vector3 rot {  get; private set; }

        public CreotlyTransforms(Vector3 p, Vector3 r)
        {
            pos = p;
            rot = r;
        }
    }

    [System.Serializable]
    public struct ImpactFXConfig
    {
        public GameObject decal;
        public ParticleSystem particle;
        public ImpactSurface impactSurface;
    }


    [System.Serializable]
    public struct BoundaryFloat
    {
        public float lowerBound;
        public float upperBound;

        public BoundaryFloat(float min, float max)
        {
            lowerBound = min;
            upperBound = max;
        }
    }

    [System.Serializable]
    public struct BoundaryInt
    {
        public int lowerBound;
        public int upperBound;

        public BoundaryInt(int min, int max)
        {
            lowerBound = min;
            upperBound = max;
        }
    }
}