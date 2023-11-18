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
    public int maxKills = 3;
    public GameObject gameOverPopup;

    public float TimerToEndMatch = 5f;
    public bool isGameOver;
    public bool isRestarting;

    [SerializeField] GameObject playerPrefab;
    GameObject[] playerSpawnPoint;
    public Photon.Realtime.Player owner;

    private List<Player> winners = new List<Player>();

    [SerializeField] GameObject disconnectPanel;
    [SerializeField] Text textDisconnected;

    private void Awake()
    {
        playerSpawnPoint = GameObject.FindAnyObjectByType<PlayerSpawnManager>().playerSpawnPoint;
    }

    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name,playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].transform.position, Quaternion.identity);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.GetScore() == maxKills && !winners.Contains(targetPlayer))
        {
            winners.Add(targetPlayer);
            
            // Declare game over when a player reaches maxKills
            isGameOver = true;
        }


        if (isGameOver)
        {
            DisplayGameOverPopup();
        }
    }

    private void DisplayGameOverPopup()
    {
        gameOverPopup.GetComponent<EndingCanvas>().UpdateInformation(winners);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
            PhotonNetwork.LocalPlayer.SetScore(maxKills);
        }
    }

    IEnumerator DisconnectPanelTimer()
    {
        yield return new WaitForSeconds(5f);
        disconnectPanel.SetActive(false);
    }
}
