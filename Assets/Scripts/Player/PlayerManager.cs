using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

    [HideInInspector]
    public bool isInteracting; // Maybe to fix this later not to be public

    [Header("Player Information")]
    bool isDead;
    float health = 100f;
    public float currentHealth;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        animator = GetComponent<Animator>();

        currentHealth = health;
    }

    private void Update()
    {
        inputManager.HandleAllInputs();

        if(currentHealth <= 0)
        {
            isDead = true;
        }
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
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            animator.SetBool("isDead", true);
        }
    }

    public bool ReportDead()
    {
        return isDead;
    }
}
