using UnityEngine;

namespace Creotly_Studios
{
    public class MeshSocket : MonoBehaviour
    {
        Transform attachPoint;

        [field: Header("Properties")]
        [SerializeField] private Vector3 offsetPosition;
        [SerializeField] private Vector3 offsetRotation;
        [SerializeField] private HumanBodyBones humanBone;
        [field: SerializeField] public MeshSockets_ID socketID { get; private set; }

        private void Start()
        {
            Animator animator = GetComponentInParent<Animator>();
            attachPoint = new GameObject("socket " + socketID).transform;

            attachPoint.SetParent(animator.GetBoneTransform(humanBone));
            Quaternion targetRotation = Quaternion.Euler(offsetRotation);
            attachPoint.SetLocalPositionAndRotation(offsetPosition, targetRotation);
        }
        
        public void AttachPoint(Transform objectTransform)
        {
            objectTransform.SetParent(attachPoint, false);
        }
    }
}
