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
    EnemyManager enemyManager;

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

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;
        playerRigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerWeapon = GetComponent<PlayerWeapon>();

        enemyManager = FindAnyObjectByType<EnemyManager>();
    }
    //Test

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
    }
    
    void Update()
    {
        FadeInInvisible();
        FadeOutInvisible();
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
        if(isInvisible == true)
        {
            isGoingVisible = true;
            isInvisible = false;
        }

        if ((isGrounded && Time.time > nextFire) && !isInvisible)
        {
            playerWeapon.Shoot(autoTarget);
        }
    }

    //Go Invisible
    public void SecondaryAttack()
    {
        if (isInvisible == false)
            isGoingInvisible = true;
    }

    void FadeInInvisible()
    {
        // TO DO
        // Add photon check to see if the current player is the owner ( https://pastebin.com/ZLmG5tZy )
        // if so run the below code
        // if not , do not send information and disable character mesh

        if (isGoingInvisible == true)
        {
            //Disable weapons & Magic mask
            foreach (GameObject element in playerDisableElements)
            {
                element.SetActive(false);
            }

            //Grab the new material
            playerBody.GetComponent<Renderer>().material = playerMaterials[1];

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
                timerInvisibleFadeIn = 0;
            }
        }
    }

    void FadeOutInvisible()
    {
        if (isGoingVisible)
        {
            //foreach (GameObject element in playerDisableElements)
            //{

            //}
            playerDisableElements[0].SetActive(true);
            playerBody.GetComponent<Renderer>().material = playerMaterials[1];

            timerVisibleFadeOut += Time.deltaTime * visibleTimer;

            shaderProperty = Shader.PropertyToID("_cutoff");
            playerBody.GetComponent<Renderer>().material.SetFloat(shaderProperty, fadeOut.Evaluate(Mathf.InverseLerp(0, fadeOutTime, timerVisibleFadeOut)));

            if (timerVisibleFadeOut >= fadeOutTime)
            {
                isGoingVisible = false;
                isInvisible = false;
                playerBody.GetComponent<Renderer>().material = playerMaterials[0];
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
}
