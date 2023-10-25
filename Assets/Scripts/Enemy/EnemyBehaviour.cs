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
    public Slider healthBar;

    //MagicAttackVFX
    public float magicFireRate = 5f;
    float nextFire;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            transform.LookAt(other.transform);

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if(distance > 3f && Time.time > nextFire)
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




}