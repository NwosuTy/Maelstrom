using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class CharacterAnimatorRigController : MonoBehaviour
    {
        private RigBuilder rigBuilder;
        private CharacterManager characterManager;

        [Header("Body Rigs")]
        [SerializeField] private Rig bodyAim_Rig;
        [SerializeField] private Rig fandIKPose_Rig;
        [SerializeField] private Rig footIKPose_Rig;

        [Header("Weapon Rigs")]
        [SerializeField] private Rig weaponAim_Rig;
        [SerializeField] private Rig weaponPose_Rig;
        [SerializeField] private Rig weaponHolder_Rig;

        [Header("Weapon Pivot Parameters")]
        [SerializeField] private Transform mainGrip;
        [SerializeField] private Transform secondaryGrip;

        [Header("Hand IK Constraints")]
        [SerializeField] private TwoBoneIKConstraint leftHandConstraint;
        [SerializeField] private TwoBoneIKConstraint rightHandConstraint;

        private void Awake()
        {
            rigBuilder = GetComponentInParent<RigBuilder>();
            characterManager = GetComponentInParent<CharacterManager>();
        }

        public void MoveCharacterHandToWeaponPlacement(Transform grip1, Transform grip2)
        {
            
        }


        public void InitializeHandConstraints()
        {
            SetHandIKConstraintTarget(1.0f, rightHandConstraint, mainGrip);
            SetHandIKConstraintTarget(0.75f, leftHandConstraint, secondaryGrip);
            rigBuilder.Build();
        }

        private void SetHandIKConstraintTarget(float weight, TwoBoneIKConstraint hand, Transform target)
        {
            if(target == null)
            {
                hand.weight = 0.0f;
                return;
            }
            hand.data.target = target;
            hand.weight = weight;
        }
    }
}
