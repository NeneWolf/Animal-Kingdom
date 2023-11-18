using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    bool hasStartedRespawn;

    [Header("Stamina Information")]
    float stamina = 100f;
    public float currentStamina;
    public float timeToRecoverStamina = 3f;
    public bool canSprint = true;
    [SerializeField] Image staminaRefillImage;


    RigidbodyConstraints originalConstraints;

    PhotonView photonView;
    Rigidbody rb;
    PlayerSpawnManager playerPointsSpawnManager;

    MultiplayerLevelManager multiplayerLevelManager;
    RespawnCanvas respawnCanvas;
    EndingCanvas endingCanvas;
    [SerializeField] float respawnTime = 10f;

    ScenesManager scenesManager;

    private void Awake()
    {
        if(transform.position.y < 0)
        {
              transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        }

        photonView = GetComponent<PhotonView>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        originalConstraints = rb.constraints;

        inputManager = GetComponent<Multi_InputManager>();
        playerLocomotion = GetComponent<Multi_PlayerLocomotion>();
        playerPointsSpawnManager = GameObject.FindAnyObjectByType<PlayerSpawnManager>().GetComponent<PlayerSpawnManager>();

        healthRefillImage = GameObject.FindGameObjectWithTag("HealthUIFill").GetComponent<Image>();
        staminaRefillImage = GameObject.FindGameObjectWithTag("StaminaUIFill").GetComponent<Image>();
        multiplayerLevelManager = GameObject.FindAnyObjectByType<MultiplayerLevelManager>().GetComponent<MultiplayerLevelManager>();
        animator = GetComponent<Animator>();

        currentHealth = health;
        currentStamina = stamina;

        if (photonView.IsMine)
        {
            respawnCanvas = GameObject.FindAnyObjectByType<RespawnCanvas>();

            cameraObject = GameObject.FindAnyObjectByType<Multi_CameraManager>().gameObject;
            if (cameraObject != null)
            {
                cameraObject.GetComponent<Multi_CameraManager>().enabled = true;
                cameraManager = cameraObject.GetComponent<Multi_CameraManager>();
                cameraManager.WakeCamera(this.gameObject);
            }

            endingCanvas = GameObject.FindAnyObjectByType<EndingCanvas>().GetComponent<EndingCanvas>();
            endingCanvas.SendInformationToEndingCanvas(this.gameObject);
        }
        else return;

    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            if (!isDead)
            {
                inputManager.HandleAllInputs();

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

            if (isDead)
            {
                if(multiplayerLevelManager.isGameOver == false && !hasStartedRespawn)
                {
                    PlayerRespawn();
                }
                else if(multiplayerLevelManager.isGameOver == true)
                {
                    respawnCanvas.DisplayCountDown(respawnTime, false,false);
                    StopCoroutine(Respawn());
                }
            }
        }
        else{ return; }

    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            //if (!isDead)
            //{
            //    playerLocomotion.HandleAllMovement();
                
            //}

            cameraManager.HandleAllCameraMovement();
        }
        else return;
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            if (!isDead)
            {
                isInteracting = animator.GetBool("isInteracting");
                playerLocomotion.isJumping = animator.GetBool("isJumping");
                
                photonView.RPC("PlayTargetAnimation", RpcTarget.AllViaServer, "isGrounded", playerLocomotion.isGrounded);
                animator.SetBool("isGrounded", playerLocomotion.isGrounded);
            }
        }
        else return;
    }

    public void TakeDamage(Multi_OrbMovement bullet)
    {
        Debug.Log("Player took damage +1");

        if(playerLocomotion.isInvisible)
            playerLocomotion.isGoingVisible = true;

        if(currentHealth - bullet.damage > 0){             
            currentHealth -= bullet.damage;
        }
        else if(currentHealth - bullet.damage <= 0)
        {
            currentHealth = 0;
            PlayerDied();
            bullet.Owner.AddScore(1);
        }
    }

    void PlayerDied()
    {
        isDead = true;

        photonView.RPC("PlayTargetAnimationOthers", RpcTarget.AllViaServer, "isDead", isDead);
        animator.SetBool("isDead", isDead);

        capsuleCollider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;

    }

    void PlayerRespawn()
    {
        StartCoroutine(Respawn());
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
        hasStartedRespawn = true;
        var ran = UnityEngine.Random.Range(0, playerPointsSpawnManager.playerSpawnPoint.Length);

        yield return new WaitForSeconds(1f);

        transform.position = playerPointsSpawnManager.playerSpawnPoint[ran].transform.position;
        transform.rotation = new Quaternion(0f,0f,0f,0f);

        photonView.RPC("PlayTargetAnimationOthers", RpcTarget.AllViaServer, "isDead", false);
        photonView.RPC("PlayTargetAnimationOthers", RpcTarget.AllViaServer, "isRespawning", true);

        animator.SetBool("isDead", false);
        animator.SetBool("isRespawning", true);

        respawnCanvas.DisplayCountDown(respawnTime, true,true);

        yield return new WaitForSeconds(respawnTime);


        photonView.RPC("PlayTargetAnimationOthers", RpcTarget.AllViaServer, "isRespawning", true);
        animator.SetBool("isRespawning", false);

        isDead = false;
        capsuleCollider.enabled = true;
        rb.constraints = originalConstraints;
        rb.useGravity = true;

        currentHealth = health;
        currentStamina = stamina;
        hasStartedRespawn = false;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(isDead);
            stream.SendNext(rb.useGravity);
            stream.SendNext(capsuleCollider.enabled);
            stream.SendNext(hasStartedRespawn);

            stream.SendNext(currentStamina);
            stream.SendNext(rb.constraints);
            stream.SendNext(isInteracting);

        }
        else
        {
            currentHealth = (float)stream.ReceiveNext();

            healthSlider.value = currentHealth;
            healthRefillImage.fillAmount = currentHealth / health;

            isDead = (bool)stream.ReceiveNext();
            rb.useGravity = (bool)stream.ReceiveNext();
            capsuleCollider.enabled = (bool)stream.ReceiveNext();

            hasStartedRespawn = (bool)stream.ReceiveNext();

            currentStamina = (float)stream.ReceiveNext();
            rb.constraints = (RigidbodyConstraints)stream.ReceiveNext();

            isInteracting = (bool)stream.ReceiveNext();
        }
    }
}
