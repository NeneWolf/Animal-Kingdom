using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpSettings : MonoBehaviour
{
    [SerializeField] Upgrades upgradesPower;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private TMP_Text powerUpName;
    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        powerUpName.text = upgradesPower.GetNameOfPowerUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(uiCanvas.activeInHierarchy == true)
        {
            uiCanvas.transform.LookAt(player.transform);
        }

        if(upgradesPower.IsDestroyed)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
            uiCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            uiCanvas.SetActive(false);
        }
    }

    
}
