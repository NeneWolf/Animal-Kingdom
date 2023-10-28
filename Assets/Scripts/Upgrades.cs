using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    [SerializeField] string powerUp;
    [SerializeField] string displayedName;
    public bool IsDestroyed;

    //AutoTarget
    [SerializeField] private float autoTargetTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch Statement for PowerUps
            switch (powerUp)
            {
                case "AutoTarget":
                    other.GetComponent<PlayerLocomotion>().HandleAutoTarget(autoTargetTimer);
                    break;
                case "Heal":
                    //Add code
                    break;
                case "Stamina":
                    //Add code
                    break;
                default:
                    break;
            }



            IsDestroyed = true;
            Destroy(gameObject);
        }
    }

    public string GetNameOfPowerUp()
    {
        return displayedName;
    }
}
