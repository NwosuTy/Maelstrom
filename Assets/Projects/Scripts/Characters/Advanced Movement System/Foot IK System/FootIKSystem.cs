using UnityEngine;
using System.Collections;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class FootIKSystem : MonoBehaviour
    {
        private int leftFootHash;
        private int rightFootHash;

        //transform created as child of footForward to adjust for relative rotation 
        private Transform _footPlacementL;
        private Transform _footPlacementR;

        //just the y component
        private Vector3 _lastIkPositionL;
        private Vector3 _lastIkPositionR;

        //for blending/lerping since values get reset every frame in animation cycle
        private Vector3 _lastPelvisPosition;
        private Quaternion _lastIkRotationL;
        private Quaternion _lastIkRotationR;

        private Animator animator;
        private bool isIKReady = false;
        private CharacterManager characterManager;

        [Header("Status")]
        [SerializeField] private bool active = true;

        [Space]
        [Header("IK")]
        [SerializeField] private Rig ikRig;
        [SerializeField] private RigBuilder rigBuilder;
        [SerializeField] private TwoBoneIKConstraint ikConstraintL;
        [SerializeField] private TwoBoneIKConstraint ikConstraintR;

        [Header("Extract Transform Constraints")]
        [SerializeField] private ExtractTransformConstraint extractConstraintL;
        [SerializeField] private ExtractTransformConstraint extractConstraintR;
        [SerializeField] private ExtractTransformConstraint extractConstraintPelvis;

        [Space]
        [Header("Transforms")]
        [Tooltip("Not Actual Pelvis Bone, Substitute in Pelvis MultiPosition Constraint (Source Object 0)")]
        [SerializeField] private Transform pelvis;
        [Tooltip("Transform Facing Forward of Left Foot")]
        [SerializeField] private Transform footForwardL;
        [Tooltip("Transform Facing Forward of Left Foot")]
        [SerializeField] private Transform footForwardR;

        [Header("Presets")]
        [Tooltip("Pelvis Offset Based on Specific Model, Adjust in Start Of Game")]
        [SerializeField] private float pelvisOffset;
        //limits for adjusting pelvis
        [SerializeField] private float maxStepHeight;
        [SerializeField] private float minStepHeight;

        [Header("Parameters")]
        [SerializeField] private float feetIkSpeed;
        [SerializeField] private float pelvisMoveSpeed;
        [SerializeField] private LayerMask walkableLayerMasks;

        private void Awake()
        {
            characterManager = GetComponent<CharacterManager>();

            //Create Child Transforms for relative Rotation IK and assign to initial foot rotation to get rotation offset
            _footPlacementL = SetFootPlacement("FootPlacementL", footForwardL, ikConstraintL);
            _footPlacementR = SetFootPlacement("FootPlacementR", footForwardR, ikConstraintR);
        }

        private void Start()
        {
            animator = characterManager.animator;
            leftFootHash = Animator.StringToHash("leftFootIK");
            rightFootHash = Animator.StringToHash("rightFootIK");

            StartCoroutine(InitializeIKAfterRigIsReady());
        }

        private IEnumerator InitializeIKAfterRigIsReady()
        {
            // Wait for 1 frame so RigBuilder and Animator finish evaluating
            yield return null;

            // Set IK target positions to tip bones
            ikConstraintL.data.target.position = ikConstraintL.data.tip.position;
            ikConstraintR.data.target.position = ikConstraintR.data.tip.position;

            rigBuilder.Build();
            isIKReady = true;
        }

        public void FootIKSystem_Update()
        {
            if(!isIKReady || characterManager.isGrounded != true || characterManager.isJumping)
            {
                return;
            }

            if(ikConstraintL.weight < 1 || ikConstraintR.weight < 1)
            {
                SetBoneIKConstraint(1f);
            }

            //Get all original Bone Positions
            Vector3 pelvisPosition = extractConstraintPelvis.data.position;
            pelvisPosition.y += pelvisOffset;

            Vector3 bonePositionL = extractConstraintL.data.position;
            Vector3 bonePositionR = extractConstraintR.data.position;

            Quaternion boneRotationL = extractConstraintL.data.rotation;
            Quaternion boneRotationR = extractConstraintR.data.rotation;

            ikRig.weight = active ? 1f : 0f;

            if (!active)
            {
                _lastPelvisPosition = pelvisPosition;

                _lastIkPositionL.y = bonePositionL.y;
                _lastIkPositionR.y = bonePositionR.y;

                _lastIkRotationL = boneRotationL;
                _lastIkRotationR = boneRotationR;

                return;
            }

            //Foot Raycast
            bool leftHit = FeetHitGround(bonePositionL, out RaycastHit hitL);
            bool rightHit = FeetHitGround(bonePositionR, out RaycastHit hitR);

            bool hit = leftHit && rightHit;

            //displacement between legs
            float delta = hitL.point.y - hitR.point.y;

            //distance between legs
            float offset = Mathf.Abs(delta);
            bool adjustPelvis = offset <= maxStepHeight && offset >= minStepHeight && hit;

            if (adjustPelvis)
            {
                //move pelvis down (always down)
                pelvisPosition.y -= offset;

                //re-adjust right foot for pelvis movement
                if (delta < 0)
                {
                    bonePositionR.y += offset;
                    boneRotationR = SolveRotation(hitR.normal, footForwardR, ref _footPlacementR);
                }
                else if (delta > 0)
                {
                    bonePositionL.y += offset;
                    boneRotationL = SolveRotation(hitL.normal, footForwardL, ref _footPlacementL);
                }
            }
            AdjustPelvis(pelvisPosition);

            //IK
            float t = feetIkSpeed * Time.deltaTime;

            //ik position
            ApplyIkPosition(ref _lastIkPositionL.y, ref ikConstraintL, bonePositionL, t);
            ApplyIkPosition(ref _lastIkPositionR.y, ref ikConstraintR, bonePositionR, t);

            //ik rotation
            SetIkRotationWeight();
            ApplyIkRotation(ref _lastIkRotationL, ref ikConstraintL, boneRotationL, t);
            ApplyIkRotation(ref _lastIkRotationR, ref ikConstraintR, boneRotationR, t);
        }

        public void SetBoneIKConstraint(float value)
        {
            ikConstraintL.weight = value;
            ikConstraintR.weight = value;
        }

        private bool FeetHitGround(Vector3 bonePos, out RaycastHit raycastHit)
        {
            Vector3 origin = bonePos + Vector3.up * maxStepHeight;
            float rayDistance = maxStepHeight * 2f;

            if (Physics.Raycast(origin, Vector3.down, out raycastHit, rayDistance, walkableLayerMasks))
            {
                return true;
            }
            raycastHit = default;
            return false;
        }

        private Transform SetFootPlacement(string objectName, Transform footForward, TwoBoneIKConstraint ikConstraint)
        {
            GameObject footPlacementObj = new(objectName);
            Transform footPlacement = footPlacementObj.transform;

            footPlacement.SetParent(footForward);
            footPlacement.localPosition = Vector3.zero;
            footPlacement.rotation = ikConstraint.data.tip.rotation;
            return footPlacement;
        }

        private void AdjustPelvis(Vector3 pelvisPosition)
        {
            _lastPelvisPosition = Vector3.Lerp(_lastPelvisPosition, pelvisPosition, pelvisMoveSpeed * Time.deltaTime);

            pelvis.position = _lastPelvisPosition;
        }

        private void ApplyIkPosition(ref float lastIkPosition, ref TwoBoneIKConstraint ikConstraint, Vector3 bonePosition, float t)
        {
            //ik position R
            lastIkPosition = Mathf.Lerp(lastIkPosition, bonePosition.y, t);

            bonePosition.y = lastIkPosition;
            ikConstraint.data.target.position = bonePosition;
        }

        private void ApplyIkRotation(ref Quaternion lastIkRotation, ref TwoBoneIKConstraint ikConstraint, Quaternion boneRotation, float t)
        {
            lastIkRotation = Quaternion.Lerp(lastIkRotation, boneRotation, t);
            ikConstraint.data.target.rotation = lastIkRotation;
        }

        private void SetIkRotationWeight()
        {
            //Get and Set Ik rotation weight
            float weightL = 1f - animator.GetFloat(leftFootHash);
            float weightR = 1f - animator.GetFloat(rightFootHash);

            ikConstraintL.data.targetRotationWeight = weightL;
            ikConstraintR.data.targetRotationWeight = weightR;
        }

        private Quaternion SolveRotation(Vector3 normal, Transform footForward, ref Transform footPlacement)
        {
            Vector3 localNormal = transform.InverseTransformDirection(normal);
            footForward.localRotation = Quaternion.FromToRotation(Vector3.up, localNormal);
            return footPlacement.rotation;
        }
    }
}