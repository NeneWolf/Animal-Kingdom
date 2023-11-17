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


    public bool isGameOver;

    [SerializeField] GameObject playerPrefab;
    GameObject[] playerSpawnPoint;
    public Photon.Realtime.Player owner;

    private List<Player> winners = new List<Player>();

    private void Awake()
    {
        playerSpawnPoint = GameObject.FindAnyObjectByType<PlayerSpawnManager>().playerSpawnPoint;
    }

    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].transform.position.x,
            0.5f, playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].transform.position.z), Quaternion.identity);
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
}
