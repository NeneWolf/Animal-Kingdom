using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    public Text roomText;
    public string roomName;

    //Logic of Joining a room that is displayed on the UI
    public void JoinRoom()
    {
        //Remove the player from the lobby
        PhotonNetwork.LeaveLobby();

        //Join the room with the room name
        PhotonNetwork.JoinRoom(roomName);
    }
}
