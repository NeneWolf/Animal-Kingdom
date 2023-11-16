using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public Photon.Realtime.Player Owner { get; private set; }

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

    public void SpawnBullet(GameObject target, Transform center, GameObject playerSelf, Photon.Realtime.Player owner)
    {
        this.target = target;
        this.centerTransform = center;
        this.playerThatSpawnProjectile = playerSelf;
        this.Owner = owner;
    }

    [PunRPC]
    void BulletMovement()
    {
        if (!hasCenterTransform)
        {
            hasCenterTransform = true;
        }

        // Calculate the new position with a downward movement on the Y-axis
        Vector3 newPosition = transform.position + transform.forward * fireSpeed * Time.deltaTime;
        newPosition.y -= 0.003f; // Adjust this value to control the speed of downward movement

        // Move towards the new position
        transform.position = Vector3.MoveTowards(transform.position, newPosition, fireSpeed * Time.deltaTime);
    }

    [PunRPC]
    void BulletMovementToTarget()
    {
        transform.LookAt(target.transform);
        transform.parent = null;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, fireSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("Collision");
        if (collision.gameObject.layer == 7)
        {
            ExplosionVFX();
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag.Equals("Player") && collision.gameObject != playerThatSpawnProjectile)
        {
            UnityEngine.Debug.Log("Hit player");
            collision.gameObject.GetComponent<Multi_PlayerManager>().TakeDamage(damage);
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
