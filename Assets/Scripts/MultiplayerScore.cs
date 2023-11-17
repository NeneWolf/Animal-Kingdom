using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerScore : MonoBehaviourPunCallbacks
{
    public GameObject playerScorePrefab;
    public Transform panel;

    Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();

    private void Start()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);

            var playerScoreObject = Instantiate(playerScorePrefab, panel);
            var playerScoreObjectText = playerScoreObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            playerScoreObjectText.text = string.Format("{0} Score: {1}", player.NickName, player.GetScore());
            playerScore[player.ActorNumber] = playerScoreObject;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        playerScoreObjectText.text = string.Format("{0} Score: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }
}