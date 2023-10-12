using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private List<GameObject> weapon;
    [SerializeField] private GameObject WeaponOrbStorage;


    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in WeaponOrbStorage.transform)
        {
            weapon.Add(child.gameObject);
        }
    }

    public void Shoot(GameObject target)
    {
        if(target != null)
        {
            if (weapon.Count > 0)
            {
                int orbSelected = Random.Range(0, weapon.Count);
                weapon[orbSelected].GetComponent<OrbBehaviour>().BulletMovementToTarget(target);
                weapon.RemoveAt(orbSelected);
            }
        else
        {
            if (weapon.Count > 0) 
            {
                int orbSelected = Random.Range(0, weapon.Count);
                weapon[orbSelected].GetComponent<OrbBehaviour>().BulletMovement();
                weapon.RemoveAt(orbSelected);
            }
        }
    }
}
