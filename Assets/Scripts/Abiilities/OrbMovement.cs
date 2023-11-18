using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    public float fireSpeed = 10f;
    [SerializeField] float damage = 10f;
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

    public void SpawnBullet(GameObject target,Transform center)
    {
        this.target = target;
        this.centerTransform = center;
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
       if (collision.gameObject.layer == 7 || collision.gameObject.tag.Equals("Enemy")) { 

            if (collision.gameObject.tag.Equals("Enemy"))
            {
                 collision.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(damage);
            }

            ExplosionVFX();
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
