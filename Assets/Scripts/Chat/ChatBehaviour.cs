using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;

public class ChatBehaviour : MonoBehaviour, IChatClientListener
{
    public string userName;
    public ChatClient chatClient;

    public InputField inputField;
    public Text chatContent;

    void Start()
    {
        chatClient = new ChatClient(this);
    }

    void Update()
    {
        chatClient.Service();   
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Chat: " + level + " - " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat State: " + state);
    }

    public void OnConnected()
    {
        chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name, creationOptions: new ChannelCreationOptions() { PublishSubscribers = true });
    }

    public void OnDisconnected()
    {
        Debug.Log(userName + " has disconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        ChatChannel currentChat;

        if(chatClient.TryGetChannel(PhotonNetwork.CurrentRoom.Name, out currentChat))
        {
            chatContent.text = currentChat.ToStringMessages();  
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();

        for(int i = 0; i < channels.Length; i++)
        {
            if (results[i])
            {
                Debug.Log("Subscribed to channel: " + channels[i] + " channel");
                chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, " has joined the chat!");
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
       // throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void SetMessage()
    {
        if(inputField.text == "")
            return;

        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, inputField.text);
        inputField.text = "";
    }
}
