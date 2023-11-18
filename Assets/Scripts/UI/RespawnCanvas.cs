using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class RespawnCanvas : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI respawnText;
    float timeToRespawn;
    bool hasStartedRespawnCount;

    public void DisplayCountDown(float timeToRespawn, bool StartCountdown, bool isDisplayed)
    {
        panel.SetActive(isDisplayed);

        this.timeToRespawn = timeToRespawn+1f;
        hasStartedRespawnCount = StartCountdown;
        respawnText.text = "Respawning in : /n" + timeToRespawn.ToString("F0");
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStartedRespawnCount)
        {
            if (timeToRespawn > 0)
            {
                timeToRespawn -= Time.deltaTime;
                respawnText.text = "Respawning in : <br>" + timeToRespawn.ToString("F0");
            }
            else
            {
                timeToRespawn = 0;
                hasStartedRespawnCount = false;
                panel.SetActive(false);
            }
        }
    }
}
