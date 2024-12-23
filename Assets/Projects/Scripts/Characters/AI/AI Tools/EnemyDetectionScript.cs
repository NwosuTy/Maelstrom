using UnityEngine;

namespace Creotly_Studios
{
    public class EnemyDetectionScript : MonoBehaviour
    {
        [Header("Manager")]
        public AIManager aIManager;

        [Header("Visual Stats")]
        public float viewAngle;
        public float viewRadius;

        [Header("Setters")]
        [Range(0f,20f)]
        public float originalViewRadius;
        [Range(0f, 360f)]
        public float originalViewAngle;

        [Header("Visual Detection Mask")]
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        private void Awake()
        {
            aIManager = GetComponent<AIManager>();
        }

        [ExecuteInEditMode]
        private void Start()
        {
            viewAngle = originalViewAngle;
            viewRadius = originalViewRadius;
        }

        public void SetSoundTarget(Sound sound)
        {
            if(aIManager.target.visualTarget != null)
            {
                return;
            }
            aIManager.target.SetDetails(TargetType.Audio, sound, null);
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if(angleIsGlobal != true)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees* Mathf.Deg2Rad));
        }
    }
}
