using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatInputManager : MonoBehaviour
{
    public GameObject chatDisplay;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Insert))
        {
            if(chatDisplay.activeSelf)
            {
                chatDisplay.SetActive(false);
            }
            else
            {
                chatDisplay.SetActive(true);
            }
        }
    }
}
