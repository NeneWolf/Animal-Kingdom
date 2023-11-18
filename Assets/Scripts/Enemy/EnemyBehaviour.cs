using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Information")]
    [SerializeField] GameObject magicAttackVFX;
    [SerializeField] GameObject positionVFX;

    [HideInInspector]
    public int health = 100;
    public float currentHealth;
    bool isDead;
    public Slider healthBar;

    //MagicAttackVFX
    public float magicFireRate = 5f;
    float nextFire;

    EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = FindAnyObjectByType<EnemyManager>();
        currentHealth = health;
    }

    private void Update()
    {
        healthBar.value = currentHealth;

        if(isDead)
        {
            enemyManager.EnemyDied();
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !other.gameObject.GetComponent<PlayerLocomotion>().isInvisible && !isDead)
        {
            transform.LookAt(other.transform);

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 3f && Time.time > nextFire)
                MagicAttack();
        }
    }

    void MagicAttack()
    {
        nextFire = Time.time + magicFireRate;
        Instantiate(magicAttackVFX, positionVFX.transform.position, positionVFX.transform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }
    }
}
