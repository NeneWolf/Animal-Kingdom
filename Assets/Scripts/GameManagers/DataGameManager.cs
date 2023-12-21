using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Leguar.TotalJSON;
using PlayFab;
using PlayFab.ClientModels;

public class DataGameManager : MonoBehaviour
{
    public static DataGameManager instance;
    [SerializeField]ProfileDataDisplay profileDataDisplay;
    public PlayerData playerData;
    public string fileName = "playerData.txt";
    public GlobalLeaderboard globalLeaderboard;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // This object will not be destroyed when loading a new scene
        }
        else
        {
            Destroy(gameObject); // If there is another instance of this object, destroy this one
        }
    }
    private void Start()
    {
        LoadPlayerData();
        LoginToPlayFab();
    }

    void LoginToPlayFab()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerData.uID,
        };

        PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginResult, PlayFabLoginError);
    }

    void PlayFabLoginResult(LoginResult loginResult)
    {
        Debug.Log("PlayFab Login Successful - " + loginResult.ToJson());
    }

    void PlayFabLoginError(PlayFabError loginError)
    {
        Debug.Log("PlayFab Login Failed - " + loginError.ErrorMessage);
    }

    public void SavePlayerData()
    {
        print("Created file: " + fileName);
        string serializedDataString = JSON.Serialize(playerData).CreateString();
        File.WriteAllText(fileName, serializedDataString);
    }

    public void LoadPlayerData()
    {
        if(!File.Exists(fileName))
        {
            playerData = new PlayerData();
            SavePlayerData();
        }
        else
        {
            string serializedDataString = File.ReadAllText(fileName);
            playerData = JSON.ParseString(serializedDataString).Deserialize<PlayerData>();
            print("Loaded file: " + fileName);
        }

        UpdateProfileData();
    }

    void UpdateProfileData()
    {
        if (profileDataDisplay != null)
        {
            profileDataDisplay.LoadProfileData();
        }
        
    }
}
