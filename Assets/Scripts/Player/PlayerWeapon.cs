using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Projectile information")]
    [SerializeField] private GameObject WeaponOrbStorage;


    [Header("Info")]
    [SerializeField] private List<GameObject> weapon;
    public int currentWeapons;


    public float projectileAttackRange;

    [Header("Target Check")]
    [SerializeField] private Transform TargetCheck;
    GameObject target;
    GameObject currentTarget;
    bool findTarget;

    [Header("Reloading")]
    [SerializeField] GameObject manaFullCharge;
    bool isReloading;
    [SerializeField] private int reloadTime;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in WeaponOrbStorage.transform)
        {
            weapon.Add(child.gameObject);
        }

        currentWeapons = weapon.Count;
    }

    private void Update()
    {
        if(currentWeapons == 0 && !isReloading)
        {
            manaFullCharge.gameObject.SetActive(false);
            StartCoroutine(WeaponReload());
        }
    }

    public void Shoot(bool autoTarget)
    {
        if (isReloading == false && currentWeapons > 0)
        {
            findTarget = autoTarget;

            if (autoTarget)
            {
                FindAutoTarget();
            }
            else
                currentTarget = null;

            weapon[currentWeapons-1].GetComponent<MeshRenderer>().enabled = false;
            weapon[currentWeapons-1].GetComponent<SphereCollider>().enabled = false;

            weapon[currentWeapons-1].GetComponent<OrbBehaviour>().FireBullet(currentTarget);
            currentWeapons--;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TargetCheck.position, projectileAttackRange);
    }

    void FindAutoTarget()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(TargetCheck.position, projectileAttackRange, transform.forward,0.5f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == "Enemy")
            {
                target = hit.transform.gameObject;

                //Compare it with the previous distance
                if (currentTarget == null)
                {
                    currentTarget = target;
                }
                else
                {
                    float distanceToNewTarget = Vector3.Distance(transform.position, target.transform.position);
                    float distanceToCurrentTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

                    if (distanceToNewTarget < distanceToCurrentTarget)
                    {
                        currentTarget = target;
                    }
                }
                break;
            }
            else
            {
                target = null;
            }
        }
    }

    IEnumerator WeaponReload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);
        foreach (GameObject orb in weapon)
        {
            orb.GetComponent<MeshRenderer>().enabled = true;
            orb.GetComponent<SphereCollider>().enabled = true;
            yield return new WaitForSeconds(reloadTime/weapon.Count);
        }
        manaFullCharge.gameObject.SetActive(true);
        currentWeapons = weapon.Count;
        isReloading = false;
    }

}