using UnityEngine;

namespace Creotly_Studios
{
    public class AnimatorEventsManager : MonoBehaviour
    {
        CharacterManager characterManager;

        AIManager aIManager;
        PlayerManager playerManager;

        // Start is called before the first frame update
        void Awake()
        {
            characterManager = GetComponent<CharacterManager>();

            aIManager = characterManager as AIManager;
            playerManager = characterManager as PlayerManager;
        }

        public void InventoryManager_EquipWeapon()
        {
            characterManager.characterInventoryManager.EquipWeapon();
        }

        public void InventoryManager_UnEquipWeapon()
        {
            characterManager.characterInventoryManager.UnEquipWeapon();
        }

        public void ApplyJumpVelocity()
        {
            if(playerManager == null)
            {
                return;
            }
            playerManager.playerLocomotionManager.ApplyJumpingVelocity();
        }

        public void ThrowGrenadeForce()
        {
            if(playerManager == null)
            {
                return;
            }
            playerManager.playerCombatManager.ThrowGrenadePhysics();
        }

        public void ApplyExplosionForce()
        {
            characterManager.characterLocomotionManager.ExplosionForce();
        }
    }
}
