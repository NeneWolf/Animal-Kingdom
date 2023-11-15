using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multi_PlayerManager : MonoBehaviour
{
    Animator animator;
    Multi_InputManager inputManager;
    Multi_PlayerLocomotion playerLocomotion;
    [SerializeField] GameObject cameraObject;
    Multi_CameraManager cameraManager;

    [SerializeField]Slider healthSlider;

    [HideInInspector]
    public bool isInteracting; // Maybe to fix this later not to be public

    [Header("Player Information")]
    bool isDead;
    float health = 100f;
    public float currentHealth;
    public Image healthRefillImage;

    [Header("Stamina Information")]
    float stamina = 100f;
    public float currentStamina;
    public float timeToRecoverStamina = 3f;
    public bool canSprint = true;
    public Image staminaRefillImage;

    PhotonView photonView;

    private void Awake()
    {
        inputManager = GetComponent<Multi_InputManager>();
        playerLocomotion = GetComponent<Multi_PlayerLocomotion>();
        photonView = GetComponent<PhotonView>();

        if (cameraObject != null)
        {
            cameraObject.GetComponent<Multi_CameraManager>().enabled = true;
            cameraManager = cameraObject.GetComponent<Multi_CameraManager>();
            cameraManager.WakeCamera();
        }

        animator = GetComponent<Animator>();

        currentHealth = health;
        currentStamina = stamina;
    }


    private void Update()
    {
        if (!photonView.IsMine)
            return;

        inputManager.HandleAllInputs();

        if(currentHealth <= 0)
        {
            isDead = true;
        }

        if(currentStamina < stamina && !playerLocomotion.isSprinting)
        {
            // Recover stamina
            currentStamina += stamina / timeToRecoverStamina * Time.deltaTime;

            if(currentStamina >= stamina)
            {
                currentStamina = stamina;
                canSprint = true;
            }
        }

        // Update UI
        healthSlider.value = currentHealth;
        staminaRefillImage.fillAmount = currentStamina / stamina;
        healthRefillImage.fillAmount = currentHealth / health;
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine)
            return;

        playerLocomotion.HandleAllMovement();
        cameraManager.HandleAllCameraMovement();
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine)
            return;

        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded",playerLocomotion.isGrounded);
    }

    public void TakeDamage(float damage)
    {
        if(playerLocomotion.isInvisible)
            playerLocomotion.isGoingVisible = true;

        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            animator.SetBool("isDead", true);
        }
    }

    public void TakeStamina(float amount)
    {
        currentStamina -= amount;
        if(currentStamina <= 0)
        {
            currentStamina = 0;
            canSprint = false;
        }
    }

    public bool ReportDead()
    {
        return isDead;
    }
}
