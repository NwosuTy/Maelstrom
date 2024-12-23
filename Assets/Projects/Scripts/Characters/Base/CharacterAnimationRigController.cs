using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class CharacterAnimationRigController : MonoBehaviour
    {
        private Transform parent;
        private Transform aimTarget;
        private CharacterManager characterManager;

        [Header("Stats")]
        [SerializeField] private float aimDuration;

        [Header("Rig Transforms")]
        [SerializeField] private Transform weaponRestIK;
        [SerializeField] private Transform weaponGripIK;

        [field: Header("Rigs")]
        public RigBuilder rigBuilder;
        [field: SerializeField] public Rig HandIKConstraints {get; private set;}
        [field: SerializeField] public Rig WeaponAimConstraint {get; private set;}

        [field: Header("Hand_IK Parameters")]
        [field: SerializeField] public TwoBoneIKConstraint LeftHandIKConstraint {get; private set;}
        [field: SerializeField] public TwoBoneIKConstraint RightHandIKConstraint {get; private set;}

        [field: Header("Aiming Constraints")]
        [field: SerializeField] public GameObject[] WeaponAimObjects { get; private set; }
        [field: SerializeField] public MultiAimConstraint[] MultiAimConstraintArray {get; private set;}

        [field: Header("Weapon Pose Constraint")]
        [field: SerializeField] public MultiParentConstraint[] WeaponParentConstraints {get; private set;}
        [field: SerializeField] public MultiPositionConstraint[] WeaponPositionConstraints {get; private set;}

        private void Awake()
        {
            rigBuilder = GetComponentInParent<RigBuilder>();

            aimTarget = GameObjectTools.FindChildObject("Aim Look At").transform;
            characterManager = GetComponentInParent<CharacterManager>();
        }

        private void Start()
        {
            parent = weaponGripIK.parent;
            foreach(MultiAimConstraint multiAimConstraint in MultiAimConstraintArray)
            {
                var sources = multiAimConstraint.data.sourceObjects;
                WeightedTransform weightedTransform = new WeightedTransform(aimTarget, 1f);

                sources.Clear();
                sources.Add(weightedTransform);
                multiAimConstraint.data.sourceObjects = sources;
            }
            rigBuilder.Build();
        }

        public void CharacterAnimationRig_Updater(float delta)
        {
            Lock_In(delta);
        }

        public void SetTwoBoneIKConstraint(Transform weaponGrip, Transform weaponRest)
        {
            LeftHandIKConstraint.weight = 0.55f;
            RightHandIKConstraint.weight = 1.0f;

            SetPositionBasedOnParent(weaponGripIK, weaponGrip);
            SetPositionBasedOnParent(weaponRestIK, weaponRest);
            rigBuilder.Build();
        }

        private void SetPositionBasedOnParent(Transform A, Transform B)
        {
            Vector3 position = parent.InverseTransformPoint(B.position);
            Quaternion rotation = Quaternion.Inverse(parent.rotation) * B.rotation;

            A.SetLocalPositionAndRotation(position, rotation);
        }

        private void Lock_In(float delta)
        {
            float moveDuration = delta / aimDuration;
            WeaponManager currentWeapon = characterManager.characterInventoryManager.currentWeaponManager;

            if(currentWeapon == null || weaponRestIK == null)
            {
                return;
            }

            if(characterManager.isLockedIn || characterManager.isAttacking)
            {
                WeaponAimConstraint.weight += moveDuration;
                weaponRestIK.localPosition = Vector3.MoveTowards(weaponRestIK.localPosition, currentWeapon.RestLockedPosition, moveDuration);
                return;
            }
            WeaponAimConstraint.weight -= moveDuration;
            weaponRestIK.localPosition = Vector3.MoveTowards(weaponRestIK.localPosition,  currentWeapon.RestOriginalPosition, moveDuration);
        }
    }
}
