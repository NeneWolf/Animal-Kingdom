using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject[] weapon;
    [SerializeField] private GameObject spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            //GameObject orb = Instantiate(weapon, spawnPoint.transform.position, transform.rotation);
            //orb.gameObject.transform.SetParent(spawnPoint.transform);
            //orb.GetComponent<OrbBehaviour>().centerObject = spawnPoint.transform;
        }
    }
}
