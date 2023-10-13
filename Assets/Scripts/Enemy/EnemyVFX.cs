using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    ParticleSystem ps;
    [SerializeField] private float damage = 10f;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ps.trigger.SetCollider(0, GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>());
    }



    private void OnParticleTrigger()
    {

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter, out var enterData);

        for (int i = 0; i < numEnter; i++)
        {
            for (int j = 0; j < enterData.GetColliderCount(i); j++)
            {
                if(enterData.GetCollider(i, j).gameObject.tag.Equals("Player"))
                {
                    enterData.GetCollider(i, j).gameObject.GetComponent<PlayerManager>().TakeDamage(damage);
                    print("Hit Player");
                }
            }
        }
    }
}
