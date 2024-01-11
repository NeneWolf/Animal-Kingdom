using Photon.Pun.Demo.PunBasics;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

public class LeaderboardPopUp : MonoBehaviour
{
    public GameObject scoreHolder;
    public GameObject noScoreText;

    public GameObject leaderboardItemPrefab;

    private void OnEnable()
    {
        DataGameManager.instance.globalLeaderboard.GetLeaderboard();
    }

    public void UpdateUI(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
    {
        if(playerLeaderboardEntries.Count > 0)
        {
            DestroyChildren(scoreHolder.transform);

            for(int i = 0; i < playerLeaderboardEntries.Count; i++)
            {
                GameObject newleaderboardItem = Instantiate(leaderboardItemPrefab, Vector3.zero,Quaternion.identity,scoreHolder.transform);
                newleaderboardItem.GetComponent<LeaderboardItem>().SetScores(i + 1, playerLeaderboardEntries[i].DisplayName, playerLeaderboardEntries[i].StatValue);
            }

            scoreHolder.SetActive(true);
            noScoreText.SetActive(false);
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreText.SetActive(true);
        }
    }

    private void DestroyChildren(Transform parent)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
