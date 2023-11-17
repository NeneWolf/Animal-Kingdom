using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCanvasToDisconnect : MonoBehaviour
{
    ScenesManager scenesManager;

    private void Awake()
    {
        scenesManager = GetComponent<ScenesManager>();
    }

    public void DisconnectButtonClick()
    {
        scenesManager.ChangeSceneByID(0);
        PhotonNetwork.Disconnect();

    }
}
