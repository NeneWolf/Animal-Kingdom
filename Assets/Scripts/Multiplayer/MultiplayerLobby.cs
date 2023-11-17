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

    //
    public  GameObject startGameButton;

    //Random Name
    private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 10000));
        cachedRoomList = new Dictionary<string, RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;
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

    //Disconnect
    public void DisconnectButtonClick()
    {
        PhotonNetwork.Disconnect();
        ActivatePanel("Login");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server.");
        ActivatePanel("Login");
    }

    //Join Room by Name
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
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
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully");
        ActivatePanel("InsideRoom");

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName;
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
        DestroyChildren(insideRoomPlayerList);
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

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a Room - " + message);
        //CreateARoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room -" + message);
    }

    //
    public override void OnPlayerEnteredRoom (Photon.Realtime.Player newPlayer)
    {
        Debug.Log("New player joined the room");


        //Similarly to the OnJoinedRoom() method where we list allthe names of the players in the room
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;

        //This line sets the display name of the object we just instantiated, (the name that wesee in the Hierarchy in Unity), to the NickName of the new player. 
        playerListEntry.name = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player left the room");

        //We have been over foreach loops in the first task of this tutorial, but we will quicklyrecap as they are very powerful.
        //Transform - is the type of the items that we have previously stored ininsideRoomPlayerList, and which the loop will iterate through.
        //child - is the name that we are giving to the items stored in insideRoomPlayerList.
        //We will only use this name to refer to these items while we are using the foreachloop.
        foreach (Transform child in insideRoomPlayerList)
        {
            if (child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGameClicked()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.CurrentRoom.PlayerCount <= 4)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("GameScene_Multiplayer");
        }

    }

    //public void OnPlayerDisconnected(PhotonNetwork player)
    //{
    //    Debug.Log("Clean up after player " + player);
    //    PhotonNetwork.DestroyPlayerObjects(player);
    //}
}
