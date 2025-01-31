using UnityEngine;
using UnityEngine.AI;

namespace Creotly_Studios
{
    public static class Maths_PhysicsHelper
    {
        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public static Vector3 SpawnPoint(float sphereRadius, Transform transform, int layerMask = NavMesh.AllAreas)
        {
            NavMeshHit navMeshHit;
            Vector3 randomPoint = Random.insideUnitSphere * sphereRadius + transform.position;
            
            if(NavMesh.SamplePosition(randomPoint, out navMeshHit, sphereRadius, layerMask))
            {
                return navMeshHit.position;
            }
            return Vector3.zero;
        }

        public static float CalculateViewAngle(Vector3 forward, Vector3 targetDirection)
        {
            targetDirection.y = 0.0f;
            float viewAngle = Vector3.Angle(forward, targetDirection);
            Vector3 cross = Vector3.Cross(forward, targetDirection);

            if(cross.y < 0.0f)
            {
                return -viewAngle;
            }
            return viewAngle;
        }
    }
}