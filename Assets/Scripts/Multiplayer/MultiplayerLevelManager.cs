using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;
using Photon.Pun.Demo.PunBasics;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{

    Player highestKillsPlayer = null;
    Player secondHighestKillsPlayer = null;
    Player winner;

    [Header("Game Information")]
    [SerializeField] GameObject timerTextGameObject;
    [SerializeField] Text timerText;
    [SerializeField] float TimerToEndMatch = 180f;
    bool isGameStarting;
    public GameObject gameOverPopup;
    public bool isGameOver;
    public bool isRestarting;

    [Header("Player Information")]
    [SerializeField] GameObject playerPrefab;
    GameObject[] playerSpawnPoint;

    [Header("Disconnect Panel")]
    [SerializeField] GameObject disconnectPanel;
    [SerializeField] Text textDisconnected;

    bool isADraw;

    public ChatInputManager chatInputManager;

    [HideInInspector]
    public Photon.Realtime.Player owner;

    private void Awake()
    {
        playerSpawnPoint = GameObject.FindAnyObjectByType<PlayerSpawnManager>().playerSpawnPoint;
    }

    void Start()
    {
        StartCoroutine(StartGameTimer());
        PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint[UnityEngine.Random.Range(0, playerSpawnPoint.Length)].transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (isGameStarting && !isGameOver)
        {
            timerTextGameObject.SetActive(true);
            TimerToEndMatch -= Time.deltaTime;

            int seconds = (int)(TimerToEndMatch % 60);
            int minutes = (int)(TimerToEndMatch / 60) % 60;

            string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
            timerText.text = timerString;
        }

        if (TimerToEndMatch <= 0)
        {
            isGameOver = true;
            timerText.text = "Time's up!";
        }

        if (isGameOver)
        {
            winner = highestKillsPlayer;
            DisplayGameOverPopup();
            StorePersonalBest();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if( targetPlayer.GetScore() > 0)
        {
            if (highestKillsPlayer == null)
            {
                highestKillsPlayer = targetPlayer;
            }
            else
            {
                if (targetPlayer.GetScore() > highestKillsPlayer.GetScore())
                {
                    highestKillsPlayer = targetPlayer;
                }
            }
        }
        else
        {
            highestKillsPlayer = null;
        }
    }

    private void DisplayGameOverPopup()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameOverPopup.GetComponent<EndingCanvas>().UpdateInformation(winner);
    }

    public void LeaveGame()
    {
        chatInputManager.DisableChat();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        chatInputManager.DisableChat();
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        disconnectPanel.SetActive(true);
        textDisconnected.text = otherPlayer.NickName + " has left the room.";
        StartCoroutine(DisconnectPanelTimer());

        if(PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            highestKillsPlayer = PhotonNetwork.LocalPlayer;
            isGameOver = true;
        }
    }

    IEnumerator DisconnectPanelTimer()
    {
        yield return new WaitForSeconds(5f);
        disconnectPanel.SetActive(false);
    }

    IEnumerator StartGameTimer()
    {
        yield return new WaitForSeconds(1f);
        isGameStarting = true;
    }

    public void RestartGame()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("LoadingSceneRestart");
    }

    void StorePersonalBest()
    {
        int currentPersonalBest = PhotonNetwork.LocalPlayer.GetScore();
        print(currentPersonalBest);
        PlayerData playerData = DataGameManager.instance.playerData;

        if(currentPersonalBest > playerData.bestScore)
        {
            playerData.username = PhotonNetwork.LocalPlayer.NickName;
            playerData.bestScore = currentPersonalBest;
            playerData.bestScoreDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
            playerData.totalPlayersInGame = PhotonNetwork.CurrentRoom.PlayerCount;
            playerData.roomName = PhotonNetwork.CurrentRoom.Name;

            DataGameManager.instance.globalLeaderboard.SubmitScore(currentPersonalBest);
            DataGameManager.instance.SavePlayerData();
        }
    }
}
