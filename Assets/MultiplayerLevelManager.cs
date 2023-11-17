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
    public Text winnerText;


    [SerializeField] GameObject playerPrefab;
    GameObject[] playerSpawnPoint;
    public Photon.Realtime.Player owner;

    private void Awake()
    {
        playerSpawnPoint = GameObject.FindFirstObjectByType<PlayerSpawnManager>().playerSpawnPoint;
    }

    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].GetComponent<SpawnPoints>().spawnPlayerPosition.position.x,
            0.5f, playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].GetComponent<SpawnPoints>().spawnPlayerPosition.position.z), Quaternion.identity);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer.GetScore() == maxKills)
        {
            winnerText.text = targetPlayer.NickName + " Wins!";
            gameOverPopup.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
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
