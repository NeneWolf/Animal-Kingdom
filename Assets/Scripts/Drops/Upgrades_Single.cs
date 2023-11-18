using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades_Single : MonoBehaviour
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
                    other.GetComponent<PlayerLocomotion>().HandleAutoTarget(autoTargetTimer);
                    break;
                case "Heal":
                    if (other.GetComponent<PlayerManager>().currentHealth + healAmount > 100)
                        other.GetComponent<PlayerManager>().currentHealth = 100;
                    else
                        other.GetComponent<PlayerManager>().currentHealth += healAmount;
                    break;
                case "Stamina":
                    if (other.GetComponent<PlayerManager>().currentStamina + staminaAmount > 100)
                        other.GetComponent<PlayerManager>().currentStamina = 100;
                    else
                        other.GetComponent<PlayerManager>().currentStamina += staminaAmount;
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
