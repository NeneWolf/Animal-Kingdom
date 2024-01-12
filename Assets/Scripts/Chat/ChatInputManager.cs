using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ChatInputManager : MonoBehaviour
{
    public GameObject chatDisplay;
    public ChatBehaviour chat;

    public GameObject chatIcon;
    public InputField chatInput;

    bool disableChat;
    float timeToDisableChat = 10f;
    float time;


    public bool isInGame;
    bool hasOpenForFirstTime;

    private void Start()
    {
        //Set timer to disable chat
        time = timeToDisableChat;

        // Disable the chat input initially
        chatInput.interactable = false;

        hasOpenForFirstTime= false;
    }

    private void Update()
    {
        //countdown to disable chat
        if (chatDisplay.activeSelf && disableChat == true)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                DisableChat();
            }
        }

        if (!chatDisplay.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            if(hasOpenForFirstTime == false && isInGame)
            {
                var authentificationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
                chat.userName = PhotonNetwork.LocalPlayer.NickName;
                chat.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authentificationValues);
                hasOpenForFirstTime = true;
            }

            OpenChat();
        }
        else if(chatDisplay.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            FocusChatInput();
        }

        if (chatInput.interactable == true && Input.GetKeyDown(KeyCode.Return))
        {
            disableChat = true;
            time = timeToDisableChat;

            chat.SetMessage();
        }
    }

    public void OpenChat()
    {
        chatDisplay.SetActive(true);

        FocusChatInput();

        //if (chatIcon != null)
        //    chatIcon.SetActive(false);
    }

    public void OpenChatOnNewMessage()
    {
        chatDisplay.SetActive(true);
        time = 5f;
        disableChat = true;
    }

    void FocusChatInput()
    {
        //Toggle to interact with the chat input
        chatInput.interactable = true;

        // Set the focus to the chat input
        if (chatInput.interactable)
        {
            chatInput.Select();
            chatInput.ActivateInputField();
        }
    }

    public void DisableChat()
    {
        time = timeToDisableChat;

        //Toggle to interact with the chat input
        chatInput.interactable = false;

        chatDisplay.SetActive(false);

        //if (chatIcon != null)
        //    chatIcon.SetActive(true);

        disableChat = false;
    }

    public void InstantDisableChat()
    {
        time = timeToDisableChat;

        //Toggle to interact with the chat input
        chatInput.interactable = false;

        chatDisplay.SetActive(false);
    }

    public void LeftGame()
    {
        //Disconnect from the chat server
        chat.chatClient.Disconnect();
    }
}
