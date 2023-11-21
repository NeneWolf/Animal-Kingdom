using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Multi_OrbMovement : MonoBehaviour
{
    public float fireSpeed = 10f;
    public float damage = 10f;
    GameObject playerThatSpawnProjectile;
    GameObject target;
    Transform centerTransform;
    bool hasCenterTransform;

    [SerializeField] private GameObject explosionVFX;

    [HideInInspector]
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

    public void SpawnBullet(Transform center, GameObject playerSelf, Photon.Realtime.Player owner)
    {
        this.target = playerSelf.GetComponent<Multi_PlayerWeapon>().currentTarget;
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
        newPosition.y -= 0.015f; // Adjust this value to control the speed of downward movement

        // Move towards the new position
        transform.position = Vector3.MoveTowards(transform.position, newPosition, fireSpeed * Time.deltaTime);
    }

    [PunRPC]
    void BulletMovementToTarget()
    {
        transform.LookAt(target.transform);
        transform.parent = null;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.transform.position.x, target.transform.position.y + 0.2f, target.transform.position.z), fireSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            ExplosionVFX();
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag.Equals("Player") && collision.gameObject != playerThatSpawnProjectile)
        {
            collision.gameObject.GetComponent<Multi_PlayerManager>().TakeDamage(this);
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
