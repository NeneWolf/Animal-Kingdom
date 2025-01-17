using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputAction playerControls;
    PlayerLocomotion playerLocomotion;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    public Vector2 moveInput;
    public float moveAmount;

    //
    public float verticalInput;
    public float horizontalInput;

    //Camera Input
    public Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;

    public bool sprintInput;
    public bool jumpInput;

    public bool isMoving;

    public bool isPrimaryAttack;
    public bool isSecondaryAttack; 
    
    EnemyManager enemyManager;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerManager = GetComponent<PlayerManager>();
        enemyManager = FindAnyObjectByType<EnemyManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInputAction();
            playerControls.PlayerMovement.Movement.performed += i => moveInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            playerControls.PlayerActions.PrimarySkill.performed += i => isPrimaryAttack = true;
            playerControls.PlayerActions.SecondSkill.performed += i => isSecondaryAttack = true;
        }

        playerControls.Enable();

    }

    //
    private void OnDisable()
    {
        playerControls.Disable();
    }

    //Handle All the inputs and calls the functions
    public void HandleAllInputs()
    {
        if (!enemyManager.isGameOver && !playerManager.ReportDead())
        {
            HandleMovementInput();
            HandleCameraInput();
            HandleSprintingInput();
            HandleJumpInput();
            PrimaryAttack();
            SecondaryAttack();
        }
    }

    private void HandleMovementInput()
    {
        verticalInput = moveInput.y;
        horizontalInput = moveInput.x;

        //if (verticalInput < 0 || horizontalInput < 0) makes it positive
        //moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        animatorManager.UpdateAnimatorValues(horizontalInput, verticalInput, playerLocomotion.isSprinting);
    }

    private void HandleCameraInput()
    {
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
    }

    private void HandleSprintingInput()
    {
        if(playerManager.canSprint)
        {
            if (sprintInput && verticalInput > 0.5f)
            {
                playerLocomotion.isSprinting = true; 
                playerManager.TakeStamina(0.05f);
            }
            else playerLocomotion.isSprinting = false;
        }
        else playerLocomotion.isSprinting = false;
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerLocomotion.HandleJump();
        }
    }

    private void PrimaryAttack()
    {
        if(isPrimaryAttack && !isSecondaryAttack)
        {
            isPrimaryAttack = false;
            playerLocomotion.PrimaryAttack();
        }
    }

    private void SecondaryAttack()
    {
        if (isSecondaryAttack && !isPrimaryAttack)
        {
            isSecondaryAttack = false;
            playerLocomotion.SecondaryAttack();
        }
    }
}
