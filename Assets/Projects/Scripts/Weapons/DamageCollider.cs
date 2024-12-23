using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class DamageCollider : MonoBehaviour
    {
        MeleeWeaponManager weaponManager;
        CharacterManager characterManager;

        public Collider[] damageColliders;
        //Maths
        private Vector3 contactPoint;
        private List<CharacterManager> characterManagersDamaged = new List<CharacterManager>();

        public void Initialize(MeleeWeaponManager weapon)
        {
            weaponManager = weapon;
            characterManager = weaponManager.characterManager;
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach(var collider in damageColliders)
            {
                collider.enabled = false;
                collider.isTrigger = true;

                weaponManager.rigidBody.isKinematic = false;
            }
        }
        
        public void EnableCollider()
        {
            foreach(var collider in damageColliders)
            {
                collider.enabled = true;
            }
        }

        public void DisableCollider()
        {
            foreach(var collider in damageColliders)
            {
                collider.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider damagedCollider)
        {
            CharacterManager damagedCharacter = damagedCollider.GetComponentInParent<CharacterManager>();
            
            if (damagedCharacter != null && damagedCharacter.characterType != characterManager.characterType)
            {
                if(damagedCharacter.isDead)
                {
                    return;
                }

                contactPoint = damagedCollider.ClosestPointOnBounds(transform.position);
                HandleDamage(contactPoint, damagedCollider, damagedCharacter);
            }
        }

        protected void HandleDamage(Vector3 contactPoint, Collider damagedCollider, CharacterManager damagedCharacter)
        {
            if(characterManagersDamaged.Contains(damagedCharacter))
            {
                return;
            }

            characterManagersDamaged.Add(damagedCharacter);
            
            MeleeDamageEffect melee_DamageEffect = characterManager.characterCombatManager.melee_DamageEffect;
            
            melee_DamageEffect.Initialize(characterManager);
            melee_DamageEffect.ProcessEffect(weaponManager.damageValue, contactPoint, damagedCollider, damagedCharacter);
        }
    }
}
