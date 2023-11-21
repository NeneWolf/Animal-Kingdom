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
    EnemyManager enemyManager;

    //[SerializeField]Slider healthSlider;

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

    Vector3 initialPosition;

    [Header("Audio")]
    [SerializeField] AudioClip hitSound;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        animator = GetComponent<Animator>();
        enemyManager = FindAnyObjectByType<EnemyManager>();

        currentHealth = health;
        currentStamina = stamina;

        initialPosition = transform.position;
    }

    private void Update()
    {
        if (!enemyManager.isGameOver)
        {
            inputManager.HandleAllInputs();

            if (currentHealth <= 0)
            {
                isDead = true;
            }

            if (currentStamina < stamina && !playerLocomotion.isSprinting)
            {
                // Recover stamina
                currentStamina += stamina / timeToRecoverStamina * Time.deltaTime;

                if (currentStamina >= stamina)
                {
                    currentStamina = stamina;
                    canSprint = true;
                }
            }

            // Update UI
            staminaRefillImage.fillAmount = currentStamina / stamina;
            healthRefillImage.fillAmount = currentHealth / health;

            if (isDead)
            {
                StartCoroutine(Respawn());
            }
        }
    }

    private void FixedUpdate()
    {
        if (!enemyManager.isGameOver)
        {
            playerLocomotion.HandleAllMovement();
            cameraManager.HandleAllCameraMovement();
        }

    }

    private void LateUpdate()
    {
        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded",playerLocomotion.isGrounded);
    }

    public void TakeDamage(float damage)
    {
        AudioManager.Instance.Play3D(hitSound, transform.position);

        if (playerLocomotion.isInvisible)
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

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        transform.position = initialPosition;

        animator.SetBool("isDead", false);
        isDead = false;
        currentHealth = health;
        currentStamina = stamina;
    }
}
