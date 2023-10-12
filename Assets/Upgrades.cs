using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    [SerializeField] private float powerTimer;

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.name);
        if(collision.collider.CompareTag("Player"))
        {

            collision.collider.GetComponent<PlayerLocomotion>().autoTarget = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerLocomotion>().HandleAutoTarget(powerTimer);
            Destroy(gameObject);
        }
    }
}
