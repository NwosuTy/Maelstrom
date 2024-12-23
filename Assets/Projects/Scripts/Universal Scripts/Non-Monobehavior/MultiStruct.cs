using System;
using UnityEngine;

namespace Creotly_Studios
{
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