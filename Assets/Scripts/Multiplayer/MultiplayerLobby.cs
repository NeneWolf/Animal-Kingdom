using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{

    public List<GameObject> panels;
    public InputField roomNameInput;

    //Player Information
    public InputField playerNameInput;
    string playerName;

    public GameObject textPrefab;
    public Transform insideRoomPlayerList;

    //
    public Transform listRoomPanel;
    public GameObject roomEntryPrefab;
    public Transform listRoomPanelContent;

    Dictionary<string, RoomInfo> cachedRoomList;

    //Random Name
    private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 10000));
        cachedRoomList = new Dictionary<string, RoomInfo>();
    }

    public void LoginButtonClicked()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;
        //Access the PhotonNetwork class and then tell it that we wish to connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        ActivatePanel("Selection");
    }

    public void ActivatePanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
        }

    }

    //Join Room by Name
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    //Disconnect
    public void DisconnectButtonClick()
    {
        PhotonNetwork.Disconnect();
        ActivatePanel("Login");
    }


    // Create Room
    public void CreateARoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message);
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully");
        ActivatePanel("InsideRoom");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
        }
    }

    // Leave room 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public override void OnLeftRoom()
    {
       Debug.Log("Left room successfully");
       ActivatePanel("CreateRoom");
       DestroyChildren(insideRoomPlayerList);
    }

    // Display Room List
    public void ListRoomClicked()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Room list updated");
        ActivatePanel("ListRooms");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);

        DestroyChildren(listRoomPanelContent);

        UpdateCachedRoomList(roomList);

        // foreach loop, where the parameter of the method:List<RoomInfo> roomList
        foreach (var room in cachedRoomList)
        {
            //creating a new instance of the GameObject prefab. Each prefab willrepresent a different room available on the master server.
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);

            //For each room entry on the list, we are getting access directly to it's RoomEntryscript. We are storing this access under a new variable newRoomEntryScript.
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Key;
            newRoomEntryScript.roomText.text = string.Format("{0} - ({1}/{2})", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers);
        }
    }

    // Leave Lobby
    public void LeaveLobbyClick()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left lobby successfully");
        DestroyChildren(listRoomPanelContent);
        cachedRoomList.Clear();
        ActivatePanel("Selection");
    }


    //
    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach(var room in roomList)
        {
            //This IF statement is going to be our conditional check. If a rooms data shows that itis closed,
            //invisible or has been removed from the list, it will be removed entirelyfrom our Dictionary.
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                cachedRoomList[room.Name] = room;
            }
        }
    }

    //Random Room
    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
