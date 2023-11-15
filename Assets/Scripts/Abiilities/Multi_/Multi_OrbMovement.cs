using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_OrbMovement : MonoBehaviour
{
    public float fireSpeed = 10f;
    [SerializeField] float damage = 10f;
    GameObject playerThatSpawnProjectile;
    GameObject target;
    Transform centerTransform;
    bool hasCenterTransform;

    [SerializeField] private GameObject explosionVFX;

    private void Awake()
    {
        StartCoroutine(DestroyBullet());
    }
    private void Update()
    {
        if (target != null)
            BulletMovementToTarget();
        else
            BulletMovement();
    }

    public void SpawnBullet(GameObject target,Transform center, GameObject playerSelf)
    {
        this.target = target;
        this.centerTransform = center;
        this.playerThatSpawnProjectile = playerSelf;
    }

    void BulletMovement()
    {
        if (!hasCenterTransform)
        {
            hasCenterTransform = true;

            // Assuming you have a reference to the camera or its transform
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Set the forward direction of the bullet to face the camera
                transform.forward = mainCamera.transform.forward;
            }
            else
            {
                // Handle the case where the camera reference is not available
                Debug.LogError("Main camera not found!");
            }
        }

        // Move towards the center of the camera
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, fireSpeed * Time.deltaTime);
    }

    void BulletMovementToTarget()
    {
        transform.LookAt(target.transform);
        transform.parent = null;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, fireSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

       if (collision.gameObject.layer == 7) 
        {
            ExplosionVFX();
            Destroy(gameObject);
        }
       else if(collision.gameObject.tag.Equals("Player") && collision.gameObject != playerThatSpawnProjectile)
        {
            collision.gameObject.GetComponent<Multi_PlayerManager>().TakeDamage(damage);
            ExplosionVFX();
            Destroy(gameObject);
        }
       else
        {
            Destroy(gameObject);
        }
    }

    private void ExplosionVFX()
    {
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5f);
        ExplosionVFX();
        Destroy(gameObject);
    }
}
