using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

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


    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        animator = GetComponent<Animator>();

        currentHealth = health;
        currentStamina = stamina;
    }

    private void Update()
    {
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
        staminaRefillImage.fillAmount = currentStamina / stamina;
        healthRefillImage.fillAmount = currentHealth / health;
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
        cameraManager.HandleAllCameraMovement();
    }

    private void LateUpdate()
    {
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
