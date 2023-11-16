using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerLevelManager : MonoBehaviour
{
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

}
