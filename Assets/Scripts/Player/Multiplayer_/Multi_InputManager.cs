using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_InputManager : MonoBehaviour, IPunObservable
{
    PlayerInputAction playerControls;
    Multi_PlayerLocomotion playerLocomotion;
    Multi_PlayerManager playerManager;
    Multi_AnimatorManager animatorManager;
    MultiplayerLevelManager multiplayerLevelManager;

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

    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        animatorManager = GetComponent<Multi_AnimatorManager>();
        playerLocomotion = GetComponent<Multi_PlayerLocomotion>();
        playerManager = GetComponent<Multi_PlayerManager>();

        multiplayerLevelManager = GameObject.FindAnyObjectByType<MultiplayerLevelManager>().GetComponent<MultiplayerLevelManager>();
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
        if (!photonView.IsMine) { return; }
        else
            playerControls.Disable();
    }

    //Handle All the inputs and calls the functions
    public void HandleAllInputs()
    {
        if (photonView.IsMine && multiplayerLevelManager.isGameOver == false)
        {
            if (!playerManager.ReportDead())
            {
                HandleMovementInput();
                HandleSprintingInput();
                HandleJumpInput();
            }

            HandleCameraInput();
        }

        if (!playerManager.ReportDead() && multiplayerLevelManager.isGameOver == false)
        {
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
        //animatorManager.UpdateAnimatorValues(horizontalInput, verticalInput, playerLocomotion.isSprinting);
        photonView.RPC("UpdateAnimatorValues", RpcTarget.AllViaServer, horizontalInput, verticalInput, playerLocomotion.isSprinting);
    }

    private void HandleCameraInput()
    {
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
    }

    private void HandleSprintingInput()
    {
        if (playerManager.canSprint)
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
        if (isPrimaryAttack && !isSecondaryAttack)
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(verticalInput);
            stream.SendNext(horizontalInput);


            stream.SendNext(sprintInput);
            stream.SendNext(jumpInput);
            stream.SendNext(isMoving);

            stream.SendNext(isPrimaryAttack);
            stream.SendNext(isSecondaryAttack);

        }
        else
        {
            verticalInput = (float)stream.ReceiveNext();
            horizontalInput = (float)stream.ReceiveNext();

            sprintInput = (bool)stream.ReceiveNext();
            jumpInput = (bool)stream.ReceiveNext();
            isMoving = (bool)stream.ReceiveNext();

            isPrimaryAttack = (bool)stream.ReceiveNext();
            isSecondaryAttack = (bool)stream.ReceiveNext();

        }
    }
}
