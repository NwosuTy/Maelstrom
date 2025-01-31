using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Creotly_Studios
{
    public class InputManager : MonoBehaviour
    {
        private PlayerManager playerManager;
        public Controls controls {get; private set;}

        //Movement Input
        private Vector2 movementInput;
        public float totalMoveAmount {get; private set;}
        public float verticalMovementInput {get; private set;}
        public float horizontalMovementInput {get; private set;}

        //Complex Locomotion
        public bool jumpInput {get; private set;}
        public bool crouchInput {get; private set;}
        public bool interactInput {get; private set;}

        //Action Input
        public bool reloadInput {get; private set;}
        public bool swapWeaponInput {get; private set;}

        //Shooting Input
        public bool attackInput {get; private set;}

        //Modifying Input
        public bool sprintInput {get; private set;}
        public bool lockedInInput {get; private set;}

        //System Input
        
        // Start is called before the first frame update
        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        private void OnEnable()
        {
            if(controls == null)
            {
                controls = new Controls();

                //Movement Input
                controls.Movement.Movement.performed += x => movementInput = x.ReadValue<Vector2>();

                //Action Inputs
                controls.GeneralActions.Jump.performed += x => jumpInput = true;
                controls.GeneralActions.Crouch.performed += x => crouchInput = true;
                controls.GeneralActions.Interact.performed += x => interactInput = true;

                //Combat Actions
                controls.CombatActions.Reload.performed += x => reloadInput = true;
                controls.CombatActions.SwapWeapon.performed += x => swapWeaponInput = true;

                //Shoot Input
                controls.CombatActions.Shoot.performed += x => attackInput = true;
                controls.CombatActions.Shoot.canceled += x => attackInput = false;

                //Modifiers
                controls.CombatActions.Aim.performed += x => lockedInInput = true;
                controls.CombatActions.Aim.canceled += x => lockedInInput = false;
                controls.GeneralActions.Sprint.performed += x => sprintInput = true;
                controls.GeneralActions.Sprint.canceled += x => sprintInput = false;
            }
            controls.Enable();
        }

        // Update is called once per frame
        public void InputManager_Updater()
        {
            HandleLockedInput();
            HandleCrouchInput();
            HandleJumpingInput();

            HandleMovementInput();
            HandleSprintingInput();
            HandleReloadingInput();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        //Functionalities

        public bool IsCurrentDeviceMouse()
        {
            //Check if there is an active mouse, and if it is in use
            if(Mouse.current != null && Mouse.current.enabled && Mouse.current.delta.ReadValue() != Vector2.zero)
            {
                return true;
            }
            return false;
        }


        public void ResetInputs()
        {
            jumpInput = false;
            crouchInput = false;
            reloadInput = false;
            swapWeaponInput = false;
            playerManager.canThrowGrenade = false;
        }

        private void HandleLockedInput()
        {
            playerManager.isLockedIn = lockedInInput;
            if(playerManager.isLockedIn)
            {
                if(jumpInput)
                {
                    playerManager.canThrowGrenade = true;
                }
            }
        }

        private void HandleReloadingInput()
        {
            if(attackInput)
            {
                return;
            }
            playerManager.canReload = reloadInput;
        }

        private void HandleJumpingInput()
        {
            if(jumpInput == true)
            {
                jumpInput = false;
                playerManager.playerLocomotionManager.HandleJumping();
            }
        }

        private void HandleCrouchInput()
        {
            if(crouchInput != true)
            {
                return;
            }

            if(playerManager.isCrouching)
            {
                playerManager.isCrouching = false;
                return;
            }
            playerManager.isCrouching = true;
        }

        private void HandleSprintingInput()
        {
            if(playerManager.playerStatsManager.currentEndurance <= playerManager.characterLocomotionManager.sprintEnduranceCost || playerManager.isCrouching || playerManager.isLockedIn)
            {
                playerManager.sprintFlag = false;
                return;
            }
            playerManager.sprintFlag = (sprintInput && totalMoveAmount >= 0.55f);
        }

        private void HandleMovementInput()
        {
            verticalMovementInput = movementInput.y;
            horizontalMovementInput = movementInput.x;

            totalMoveAmount = Mathf.Clamp01(Mathf.Abs(verticalMovementInput) + MathF.Abs(horizontalMovementInput));
            playerManager.isMoving = (totalMoveAmount > 0.0f);
        }
    }
}
