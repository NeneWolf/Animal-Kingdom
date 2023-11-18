using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{

    Player highestKillsPlayer = null;
    Player secondHighestKillsPlayer = null;
    Player winner;

    [Header("Game Information")]
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

    [HideInInspector]
    public Photon.Realtime.Player owner;

    private void Awake()
    {
        playerSpawnPoint = GameObject.FindAnyObjectByType<PlayerSpawnManager>().playerSpawnPoint;
    }

    void Start()
    {
        StartCoroutine(StartGameTimer());
        PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (isGameStarting && !isGameOver)
        {
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
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
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
                isADraw = false;
            }
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
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
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
}