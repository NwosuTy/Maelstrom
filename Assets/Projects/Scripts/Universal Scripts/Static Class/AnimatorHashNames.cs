using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public static class AnimatorHashNames
    {
        #region Animator Parameters

        //Movement
        public static int movingHash;
        public static int canRotateHash;
        public static int isGroundedHash;
        public static int rootMotionRotateHash;

        //Actions
        public static int interactHash;
        public static int isReloadingHash;
        public static int fallingTimerHash;

        #endregion

        #region Animator State Names
        //Actions
        public static int equipWeapon;
        public static int unEquipWeapon;
        public static int throwingObjectHash;
        public static int reloadingHash = Animator.StringToHash("Reloading");

        //Complex Movement
        public static int jumpHash;

        //Turning
        public static int turn_L_90;
        public static int turn_R_90;
        public static int turn_L_180;
        public static int turn_R_180;

        //Death Animations
        public static int backDeathHash;
        public static int frontDeathHash;
        public static int backExplosionDeathHash;
        public static int frontExplosionDeathHash;

        //Damage Animations
        public static int damageFrontHash;
        public static int damageBacksideHash;
        public static int damageLeftSideHash;
        public static int damageRightSideHash;

        //Explosion Damage Animations
        public static int explosionBackAnimation;
        public static int explosionFrontAnimation;

        #endregion

        public static int ConvertToHash(string parameterName)
        {
            return Animator.StringToHash(parameterName);
        }

        public static int DeathAnimation(float hitDirection)
        {
            if(hitDirection > 0)
            {
                return frontDeathHash;
            }
            return backDeathHash;
        }

        public static int ExplosionAnimation(int front, int back, float hitDirection)
        {
            if(hitDirection > 0)
            {
                return front;
            }
            return back;
        }

        public static int DamageTargetAnimation(float angleHitFrom)
        {
            if(angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                return damageFrontHash;
            }
            else if(angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                return damageFrontHash;
            }
            else if (angleHitFrom <= 45 && angleHitFrom >= -45)
            {
               return damageBacksideHash;
            }
            else if (angleHitFrom <= -44 && angleHitFrom >= -144)
            {
                return damageRightSideHash;
            }
            return damageLeftSideHash;
        }

        public static void StringsToHash()
        {
            //Movement
            movingHash = Animator.StringToHash("isMoving");
            canRotateHash = Animator.StringToHash("canRotate");
            isGroundedHash = Animator.StringToHash("isGrounded");
            rootMotionRotateHash = Animator.StringToHash("rootMotionRotation");

            //Actions
            interactHash = Animator.StringToHash("isInteracting");
            isReloadingHash = Animator.StringToHash("isReloading");
            fallingTimerHash = Animator.StringToHash("fallingTimer");

            //Actions
            equipWeapon = Animator.StringToHash("Equip Weapon");
            unEquipWeapon = Animator.StringToHash("UnEquip Weapon");
            throwingObjectHash = Animator.StringToHash("Throw Object");
            
            //Complex Movement
            jumpHash = Animator.StringToHash("Jump_Start");

            //Death Animations
            backDeathHash = Animator.StringToHash("Death_Back");
            frontDeathHash = Animator.StringToHash("Death_Front");
            backExplosionDeathHash = Animator.StringToHash("Explosion_DeathBack");
            frontExplosionDeathHash = Animator.StringToHash("Explosion_DeathFront");

            //Damage Animations
            damageFrontHash = Animator.StringToHash("Front Damage");
            damageBacksideHash = Animator.StringToHash("Back Damage");
            damageLeftSideHash = Animator.StringToHash("LeftSide Damage");
            damageRightSideHash = Animator.StringToHash("RightSide Damage");

            //Explosion Damage Animations
            explosionBackAnimation = Animator.StringToHash("Explosion_Back");
            explosionFrontAnimation = Animator.StringToHash("Explosion_Front");

            //Turning
            turn_L_90 = Animator.StringToHash("Turn_90_L");
            turn_R_90 = Animator.StringToHash("Turn_90_R");
            turn_L_180 = Animator.StringToHash("Turn_180_L");
            turn_R_180 = Animator.StringToHash("Turn_180_R");
        }
    }
}