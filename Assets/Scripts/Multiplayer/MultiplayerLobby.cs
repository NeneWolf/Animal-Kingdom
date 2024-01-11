using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject MenuCanvas;
    [SerializeField] GameObject Profile;
    [SerializeField] GameObject CreditsCanvas;

    public List<GameObject> panels;
    public InputField roomNameInput;

    [Header("Player Information")]
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

    //Chat
    public ChatBehaviour chat;
    public GameObject chatPanel;


    //Random Name
    private void Start()
    {
        //Generate a random name to be displayed in the input field
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 10000));

        //Initialize the Dictionary of the room list
        cachedRoomList = new Dictionary<string, RoomInfo>();

        //Automatically sync scene
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void LoginButtonClicked()
    {
        //Set the local player name to the value of the input field
        PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;

        //Access the PhotonNetwork class and then tell it that we wish to connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    //Called after the local player is connected to the master server
    public override void OnConnectedToMaster()
    {
        //Calls to turn on the correct panel - Selection - 
        ActivatePanel("Selection");
    }

    //Activates the panel that is passed in as a parameter, and deactivates all other panels
    public void ActivatePanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(true);

                if(panel.name == "InsideRoom")
                {
                    chatPanel.SetActive(true);
                }
                else
                {
                    chatPanel.SetActive(false);
                }
            }
            else
            {
                panel.SetActive(false);
                chatPanel.SetActive(false);

            }
        }

    }

    //Disconnect from the server
    public void DisconnectButtonClick()
    {
        //Disconnect from the Photon server
        PhotonNetwork.Disconnect();

        //Turn on the Login panel back
        ActivatePanel("Login");
    }

    //Photon Callbacks if the local player fails to connect (or disconnects) to the master server
    public override void OnDisconnected(DisconnectCause cause)
    {
        ActivatePanel("Login");
    }


    //Room
    //Join Room by Name
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // Create a room 
    public void CreateARoom()
    {
        //Create a new room with the name entered in the input field
        RoomOptions roomOptions = new RoomOptions();
        
        //Set the maximum number of players that can join the room to 4
        roomOptions.MaxPlayers = 4;

        //Set the room to visible in the lobby
        roomOptions.IsVisible = true;

        //Creates the room on the network with the room name and room settings
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    //Callbacks for creating a room
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully");
    }

    //Callbacks when the attempt to create a room fails
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message);
    }

    //Callbacks when joining a room
    public override void OnJoinedRoom()
    {
        //Chat
        var authentificationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chat.userName = PhotonNetwork.LocalPlayer.NickName;
        chat.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authentificationValues);


        ActivatePanel("InsideRoom");

        //Ensures tha only the owner ( master client) can start the game
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

    //Destroy the player information in the room
    public void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    
    //Updates the information of the room -  the number of players in the room
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

    //Callbacks when the player leaves the room
    public override void OnLeftRoom()
    {
        //Disconnect from the chat server
       chat.chatClient.Disconnect();

       ActivatePanel("CreateRoom");
       DestroyChildren(insideRoomPlayerList);
    }

    // Display Room List
    public void ListRoomClicked()
    {
        PhotonNetwork.JoinLobby();
    }

    // the list based on the room status
    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
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


    //Lobby
    //On Joining the server lobby
    public override void OnJoinedLobby()
    {
        ActivatePanel("ListRooms");
    }

    // Leave Lobby
    public void LeaveLobbyClick()
    {
        PhotonNetwork.LeaveLobby();
    }

    //Runs when the player leaves the lobby
    public override void OnLeftLobby()
    {
        //Clean the lists
        DestroyChildren(listRoomPanelContent);
        DestroyChildren(insideRoomPlayerList);
        cachedRoomList.Clear();

        //Activates the panels
        ActivatePanel("Selection");
    }


    //Random Room
    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room -" + message);
    }

    //When a player has entered the room
    public override void OnPlayerEnteredRoom (Photon.Realtime.Player newPlayer)
    {
        //Similarly to the OnJoinedRoom() method where we list all the names of the players in the room
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;

        //This line sets the display name of the object we just instantiated, (the name that wesee in the Hierarchy in Unity), to the NickName of the new player. 
        playerListEntry.name = newPlayer.NickName;
    }


    //General
    //When the player has left the room
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

    //Switching Master client in the room
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGameClicked()
    {
        //Checks if the number of players in the room is between 2 and 4
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.CurrentRoom.PlayerCount <= 4)
        {
            //Update the room properties
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            //Load the game scene
            PhotonNetwork.LoadLevel("GameScene_Multiplayer");
        }

    }

    public void ReturnToMainMenu()
    {
        MenuCanvas.SetActive(true);
        this.gameObject.SetActive(false);

        if (CreditsCanvas.activeSelf == true)
            CreditsCanvas.SetActive(false);
    }

    public void CreditsButton()
    {
        CreditsCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
    }

    //
    public void OpenProfile()
    {
        MenuCanvas.SetActive(false);
        Profile.SetActive(true);

    }

    public void CloseProfile()
    {
        MenuCanvas.SetActive(true);
        Profile.SetActive(false);
    }
}
