using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputAction playerControls;
    PlayerLocomotion playerLocomotion;
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

    public bool bInput;
    public bool jumpInput;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInputAction();
            playerControls.PlayerMovement.Movement.performed += i => moveInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.B.performed += i => bInput = true;
            playerControls.PlayerActions.B.canceled += i => bInput = false;

            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
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
        HandleMovementInput();
        HandleCameraInput();
        HandleSprintingInput();
        HandleJumpInput();
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
        if(bInput && verticalInput > 0.5f) playerLocomotion.isSprinting = true;
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
}
