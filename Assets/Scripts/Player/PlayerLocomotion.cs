using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlayerLocomotion : MonoBehaviour
{


    int shaderProperty;

    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    PlayerWeapon playerWeapon;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isGoingInvisible;
    public bool isInvisible;

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

    [SerializeField] GameObject[] playerMaterials;
    [SerializeField] GameObject[] playerDisableElements;

    public float fadeInTime;
    public float invisibleTimer;
    float timerInvisbleFadeIn;
    public float timerInvisbleFadeOut;
    public AnimationCurve fadeIn;

    [Header("PowerUps")]
    [SerializeField] private GameObject powerUpEffect;
    public bool autoTarget;
    public float timer;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;
        playerRigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }
    //Test

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
    }
    
    void Update()
    {
        FadeInInvisible();

    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
            return;
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
                animatorManager.PlayTargetAnimation("Wolf_Fall", true);

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.5f, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Wolf_Land", true);
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
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Wolf_Jump_Forward", false);
        }
        else if (isGrounded && isSprinting)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Wolf_Sprint_Jump", false);
        }
    }

    public void PrimaryAttack()
    {
        if ((isGrounded && Time.time > nextFire) && !isInvisible)
        {
            playerWeapon.Shoot(autoTarget);
        }
    }

    //Go Invisible
    public void SecondaryAttack()
    {
        isGoingInvisible = true;
    }

    void FadeInInvisible()
    {
        // Add Timer to go invisible

        //
        if (isGoingInvisible)
        {
            playerMaterials[0].SetActive(false);
            playerMaterials[1].SetActive(true);



            timerInvisbleFadeIn += Time.deltaTime * invisibleTimer;

            shaderProperty = Shader.PropertyToID("_cutoff");
            playerMaterials[1].GetComponent<Renderer>().material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, fadeInTime, timerInvisbleFadeIn)));

            if (timerInvisbleFadeIn > fadeInTime)
            {
                isGoingInvisible = false;
                isInvisible= true;
                foreach (GameObject element in playerDisableElements)
                {
                    element.SetActive(false);
                }

                playerMaterials[1].SetActive(false);
                playerMaterials[2].SetActive(true);
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
}
