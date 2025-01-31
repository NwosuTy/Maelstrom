using UnityEngine;

namespace Creotly_Studios
{
    [System.Serializable]
    public class Target
    {
        //Private Parameters
        public float Age {get {return Time.time - lastDetected;} }

        [field: Header("Source")]
        [field: SerializeField] public Transform source {get; set;}
        [field: SerializeField] public Sound audioTarget {get; set;}
        [field: SerializeField] public CharacterManager visualTarget {get; set;}

        [field: Header("Float Parameters")]
        [field: SerializeField] public float targetScore {get; set;}
        [field: SerializeField] public float lastDetected {get; set;}
        [field: SerializeField] public float targetDistance {get; set;}
        [field: SerializeField] public float targetDetectAngle {get; set;}

        [field: Header("Vector3 Parameters")]
        [field: SerializeField] public Vector3 targetPosition {get; set;}
        [field: SerializeField] public Vector3 targetDirection {get; set;}

        [field: Header("Status")]
        [field: SerializeField] public TargetType targetType {get; set;}

        public void SetDetails(TargetType tt, Sound sound, CharacterManager visual)
        {
            targetType = tt;
            audioTarget = sound;
            visualTarget = visual;
        }

        private Transform GetSource()
        {
            if(audioTarget == null && visualTarget == null)
            {
                return null;
            }

            if(targetType == TargetType.Audio)
            {
                return audioTarget.soundSource;
            }
            return visualTarget.transform;
        }

        public void ClearDetails()
        {
            source = null;
            audioTarget = null;
            visualTarget = null;

            targetScore = 0.0f;
            lastDetected = 0.0f;
            targetDistance = 0.0f;
            targetDetectAngle = 0.0f;

            targetPosition = Vector3.zero;
            targetDirection = Vector3.zero;
        }

        public void CalculateParameters(Transform aiManagerDetecting)
        {
            source = GetSource();

            if(source == null)
            {
                return;
            }
            lastDetected = Time.time;
            targetPosition = source.position;
            targetDirection = (aiManagerDetecting.position - targetPosition);

            targetDistance = targetDirection.magnitude;
            targetDetectAngle = Maths_PhysicsHelper.CalculateViewAngle(aiManagerDetecting.forward, targetDirection);
        }
    }
}
