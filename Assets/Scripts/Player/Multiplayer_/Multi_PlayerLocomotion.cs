using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Multi_PlayerLocomotion : MonoBehaviour, IPunObservable
{
    int shaderProperty;

    Multi_InputManager inputManager;
    Multi_PlayerManager playerManager;
    Multi_AnimatorManager animatorManager;
    Multi_PlayerWeapon playerWeapon;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isGoingInvisible;
    public bool isInvisible;
    public bool isGoingVisible;

    [Header("Falling")]
    [SerializeField]Transform groundChecker;
    public LayerMask groundLayer;
    public float rayCastHeightOffSet;
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;


    [Header("Secondary Skill - Fire")]
    public float fireRate = 0.75f;
    float nextFire;

    [Header("Secondary Skill - Invisble")]
    [SerializeField] GameObject playerBody;
    [SerializeField] Material[] playerMaterials;
    [SerializeField] GameObject[] playerDisableElements;

    public float fadeInTime;
    public float invisibleTimer;
    float timerInvisibleFadeIn;
    public AnimationCurve fadeIn;

    //
    public float fadeOutTime;
    public float visibleTimer;
    float timerVisibleFadeOut;
    public AnimationCurve fadeOut;

    [Header("PowerUps")]
    [SerializeField] private GameObject powerUpEffect;
    public bool autoTarget;
    public float timer;

    [Header("CanvasForOtherPlayers")]
    public GameObject canvas;

    [Header("PhotonViewData")]
    int currentMaterial;
    [SerializeField] GameObject playerMagicPA;

    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        inputManager = GetComponent<Multi_InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponent<Multi_PlayerManager>();
        animatorManager = GetComponent<Multi_AnimatorManager>();
        playerWeapon = GetComponent<Multi_PlayerWeapon>();


        currentMaterial = 0;
    }

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
    }
    
    void Update()
    {
        if (!playerManager.ReportDead())
        {
            FadeInInvisible();
            FadeOutInvisible();
        }

        if (!photonView.IsMine)
        {
            canvas.SetActive(!isInvisible);
            return;
        }
        else
        {
            canvas.SetActive(false);
        } 
    }

    public void HandleAllMovement()
    {
        if (photonView.IsMine)
        {
            if (!playerManager.ReportDead())
            {
                HandleFallingAndLanding();

                if (playerManager.isInteracting)
                    return;
            }

        }
        else return;
    }

    #region OldMovement
    //private void HandleMovement()
    //{
    //    if (isJumping) return;

    //    moveDirection = transform.forward * inputManager.verticalInput;
    //    moveDirection = moveDirection + transform.right * inputManager.horizontalInput;
    //    moveDirection.Normalize();
    //    moveDirection.y = 0;

    //    if (isSprinting)
    //    {
    //        moveDirection *= sprintingSpeed;
    //    }
    //    else
    //    {
    //        //if we sprinting /running/walking (joinstick)
    //        if (inputManager.moveAmount >= 0.5f) moveDirection *= runningSpeed;
    //        else moveDirection *= walkingSpeed;
    //    }
    //    Vector3 movementVelocity = moveDirection;
    //    playerRigidbody.velocity = movementVelocity;
    //}

    //private void HandleRotation()
    //{
    //    if (isJumping)
    //        return;

    //    float horizontalInput = inputManager.horizontalInput;

    //    if (Mathf.Abs(horizontalInput) > 0.1f)
    //    {
    //        float verticalInput = inputManager.verticalInput;

    //        //Calculate the input direction relative to the camera
    //        Vector3 cameraForward = cameraObject.forward;
    //        Vector3 cameraRight = cameraObject.right;
    //        cameraForward.y = 0;
    //        cameraRight.y = 0;
    //        cameraForward.Normalize();
    //        cameraRight.Normalize();

    //        Vector3 targetDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
    //        if (targetDirection == Vector3.zero)
    //        targetDirection = transform.forward;

    //        // Calculate the target rotation
    //        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

    //        //Smoothly interpolate between the current rotation and the target rotation
    //        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

    //        //Apply the new rotation
    //        transform.rotation = playerRotation;
    //    }
    //}
    #endregion

    public void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = groundChecker.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                photonView.RPC("PlayTargetAnimation", RpcTarget.AllViaServer, "Wolf_Fall", true);
            }
                //animatorManager.PlayTargetAnimation("Wolf_Fall", true);

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.5f, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                photonView.RPC("PlayTargetAnimation", RpcTarget.AllViaServer, "Wolf_Land", true);
                //animatorManager.PlayTargetAnimation("Wolf_Land", true);
            }

            inAirTimer = 0;
            isGrounded = true;
            playerManager.isInteracting = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJump()
    {
        if (isGrounded && !isSprinting)
        {
            //isJumping = true;
            animatorManager.animator.SetBool("isJumping", true);
            photonView.RPC("PlayTargetAnimation", RpcTarget.AllViaServer, "Wolf_Jump_Forward", false);
            //animatorManager.PlayTargetAnimation("Wolf_Jump_Forward", false);
        }
        else if (isGrounded && isSprinting)
        {
            //isJumping = true;
            animatorManager.animator.SetBool("isJumping", true);
            photonView.RPC("PlayTargetAnimation", RpcTarget.AllViaServer, "Wolf_Sprint_Jump", false);
            //animatorManager.PlayTargetAnimation("Wolf_Sprint_Jump", false);
        }
    }

    public void PrimaryAttack()
    {
        if (!playerManager.ReportDead())
        {
            if (isInvisible == true)
            {
                isGoingVisible = true;
                isInvisible = false;
            }

            if ((isGrounded && Time.time > nextFire) && !isInvisible)
            {
                //photonView.RPC("Shoot", RpcTarget.All,autoTarget);
                playerWeapon.Shoot(autoTarget);
            }
        }

    }

    //Go Invisible
    public void SecondaryAttack()
    {
        if (!playerManager.ReportDead())
        {
            if (isInvisible == false)
                isGoingInvisible = true;
        }

    }

    void FadeInInvisible()
    {
        if (isGoingInvisible == true)
        {
            //Disable weapons & Magic mask
            foreach (GameObject element in playerDisableElements)
            {
                element.SetActive(false);
            }

            //Grab the new material
            playerBody.GetComponent<Renderer>().material = playerMaterials[1];
            currentMaterial = 1;

            //Start Timer
            timerInvisibleFadeIn += Time.deltaTime * invisibleTimer;

            //Set the shader property
            shaderProperty = Shader.PropertyToID("_cutoff");
            playerBody.GetComponent<Renderer>().material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, fadeInTime, timerInvisibleFadeIn)));

            //Timer and actions when timer is done
            if (timerInvisibleFadeIn >= fadeInTime)
            {
                isGoingInvisible = false;
                isInvisible = true;

                playerBody.GetComponent<Renderer>().material = playerMaterials[2];

                currentMaterial = 3;

                timerInvisibleFadeIn = 0;
            }
        }
    }

    void FadeOutInvisible()
    {
        if (isGoingVisible)
        {
            playerDisableElements[0].SetActive(true);
            playerBody.GetComponent<Renderer>().material = playerMaterials[1];
            currentMaterial = 1;
            timerVisibleFadeOut += Time.deltaTime * visibleTimer;

            shaderProperty = Shader.PropertyToID("_cutoff");
            playerBody.GetComponent<Renderer>().material.SetFloat(shaderProperty, fadeOut.Evaluate(Mathf.InverseLerp(0, fadeOutTime, timerVisibleFadeOut)));

            if (timerVisibleFadeOut >= fadeOutTime)
            {
                isGoingVisible = false;
                isInvisible = false;

                playerBody.GetComponent<Renderer>().material = playerMaterials[0];
                currentMaterial = 0;
                timerVisibleFadeOut = 0;
            }
        }
    }

    public void HandleAutoTarget(float timer)
    {
        autoTarget = true;
        this.timer = timer;
        StartCoroutine(PlayEffect());
        StartCoroutine(AutoTargetTimer());
    }

    IEnumerator AutoTargetTimer()
    {
        yield return new WaitForSeconds(timer);
        autoTarget = false;
    }

    IEnumerator PlayEffect()
    {
        powerUpEffect.SetActive(true);
        yield return new WaitForSeconds(2f);
        powerUpEffect.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(isGoingInvisible);
            stream.SendNext(isInvisible);
            stream.SendNext(isGoingVisible);

            stream.SendNext(currentMaterial);
            stream.SendNext(timerVisibleFadeOut);
            stream.SendNext(timerInvisibleFadeIn);

            stream.SendNext(playerMagicPA.activeInHierarchy);
            stream.SendNext(fadeOutTime);
            stream.SendNext(fadeInTime);

            stream.SendNext(autoTarget);
            stream.SendNext(powerUpEffect.activeInHierarchy);
            stream.SendNext(isGrounded);
            stream.SendNext(isJumping);
            stream.SendNext(timer);

            

        }
        else
        {
            isGoingInvisible = (bool)stream.ReceiveNext();
            isInvisible = (bool)stream.ReceiveNext();
            isGoingVisible = (bool)stream.ReceiveNext();

            playerBody.GetComponent<Renderer>().material = playerMaterials[(int)stream.ReceiveNext()];
            timerVisibleFadeOut = (float)stream.ReceiveNext();
            timerInvisibleFadeIn = (float)stream.ReceiveNext();

            playerMagicPA.SetActive((bool)stream.ReceiveNext());
            fadeOutTime = (float)stream.ReceiveNext();
            fadeInTime = (float)stream.ReceiveNext();

            autoTarget = (bool)stream.ReceiveNext();
            powerUpEffect.SetActive((bool)stream.ReceiveNext());
            isGrounded = (bool)stream.ReceiveNext();
            isJumping = (bool)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();

        }
    }
}
