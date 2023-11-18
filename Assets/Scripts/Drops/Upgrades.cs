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

    [SerializeField] private float healAmount;
    [SerializeField] private float staminaAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch Statement for PowerUps
            switch (powerUp)
            {
                case "AutoTarget":
                    other.GetComponent<Multi_PlayerLocomotion>().HandleAutoTarget(autoTargetTimer);
                    break;
                case "Heal":
                        if(other.GetComponent<Multi_PlayerManager>().currentHealth + healAmount > 100)
                            other.GetComponent<Multi_PlayerManager>().currentHealth = 100;
                        else
                            other.GetComponent<Multi_PlayerManager>().currentHealth += healAmount;
                    break;
                case "Stamina":
                        if (other.GetComponent<Multi_PlayerManager>().currentStamina + staminaAmount > 100)
                            other.GetComponent<Multi_PlayerManager>().currentStamina = 100;
                        else
                            other.GetComponent<Multi_PlayerManager>().currentStamina += staminaAmount;
                    break;
                default:
                    break;
            }

            IsDestroyed = true;
            StartCoroutine(DestroyPowerUp());
        }
    }

    public string GetNameOfPowerUp()
    {
        return displayedName;
    }

    IEnumerator DestroyPowerUp()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
