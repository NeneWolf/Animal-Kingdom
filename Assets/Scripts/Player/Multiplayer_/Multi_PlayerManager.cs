using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Multi_PlayerManager : MonoBehaviour, IPunObservable
{
    Animator animator;
    Multi_InputManager inputManager;
    Multi_PlayerLocomotion playerLocomotion;
    public GameObject cameraObject;
    Multi_CameraManager cameraManager;

    [SerializeField]Slider healthSlider;

    [HideInInspector]
    public bool isInteracting; // Maybe to fix this later not to be public

    [Header("Player Information")]
    bool isDead;
    float health = 100f;
    public float currentHealth;
    [SerializeField] Image healthRefillImage;
    CapsuleCollider capsuleCollider;

    [Header("Stamina Information")]
    float stamina = 100f;
    public float currentStamina;
    public float timeToRecoverStamina = 3f;
    public bool canSprint = true;
    [SerializeField] Image staminaRefillImage;

    PhotonView photonView;

    private void Awake()
    {
        inputManager = GetComponent<Multi_InputManager>();
        playerLocomotion = GetComponent<Multi_PlayerLocomotion>();
        photonView = GetComponent<PhotonView>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        healthRefillImage = GameObject.FindGameObjectWithTag("HealthUIFill").GetComponent<Image>();
        staminaRefillImage = GameObject.FindGameObjectWithTag("StaminaUIFill").GetComponent<Image>();

        if (photonView.IsMine)
        {
            cameraObject = GameObject.FindAnyObjectByType<Multi_CameraManager>().gameObject;
            if (cameraObject != null)
            {
                cameraObject.GetComponent<Multi_CameraManager>().enabled = true;
                cameraManager = cameraObject.GetComponent<Multi_CameraManager>();
                cameraManager.WakeCamera(this.gameObject);
            }

            animator = GetComponent<Animator>();

            currentHealth = health;
            currentStamina = stamina;
        }
        else return;
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            inputManager.HandleAllInputs();

            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetBool("isDead", true);
                capsuleCollider.enabled = false;
            }
            else if(currentHealth > 0)
            {
                capsuleCollider.enabled = true;
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
            healthSlider.value = currentHealth;
            staminaRefillImage.fillAmount = currentStamina / stamina;
            healthRefillImage.fillAmount = currentHealth / health;
        }
        else{ return; }

    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            playerLocomotion.HandleAllMovement();
            cameraManager.HandleAllCameraMovement();
        }
        else return;
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            isInteracting = animator.GetBool("isInteracting");
            playerLocomotion.isJumping = animator.GetBool("isJumping");
            animator.SetBool("isGrounded", playerLocomotion.isGrounded);
        }
        else return;

    }

    public void TakeDamage(float damage)
    {
        if(playerLocomotion.isInvisible)
            playerLocomotion.isGoingVisible = true;

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(isDead);
        }
        else
        {
            currentHealth = (float)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
        }
    }
}
