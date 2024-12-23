using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Melee Damage Effect" , menuName = "DamageEffect/Melee Damage Effect" )]
    public class MeleeDamageEffect : ScriptableObject
    {
        CharacterManager characterCausingDamage;

        //Parameters
        private float angleHitFrom;

        //Animations
        private int deadAnimation;
        private int damageAnimation;

        public void Initialize(CharacterManager characterManager)
        {
            characterCausingDamage = characterManager;
        }

        private float CalculateAngleOfHit(CharacterManager damagedManager)
        {
            float directionFromHit = Vector3.SignedAngle(characterCausingDamage.transform.position, damagedManager.transform.position, Vector3.up);
            return directionFromHit;
        }

        public void ProcessEffect(float damageScore, Vector3 contactPoint, Collider damagedCollider, CharacterManager damagedCharacter)
        {
            if(damagedCharacter.isDead)
            {
                return;
            }

            PlayDamageSFX(damagedCharacter);
            DoDamage(damageScore, damagedCharacter);
            PlayDamagedVFX(contactPoint, damagedCharacter);
        }

        private void PlayDamageSFX(CharacterManager damagedCharacter)
        {

        }

        private void PlayDamagedVFX(Vector3 contactPoint, CharacterManager damagedCharacter)
        {
            
        }

        private void DoDamage(float damageValue, CharacterManager damagedCharacter)
        {
            angleHitFrom = CalculateAngleOfHit(damagedCharacter); 
            damageAnimation = AnimatorHashNames.DamageTargetAnimation(angleHitFrom);
            damagedCharacter.characterStatsManager.TakeHealthDamage(damageAnimation, deadAnimation, damageValue);
        }
    }
}
