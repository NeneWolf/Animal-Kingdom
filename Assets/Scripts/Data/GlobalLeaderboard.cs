using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class GlobalLeaderboard : MonoBehaviour
{
    int maxResultsCount = 5;
    public LeaderboardPopUp leaderboardPopUp;

    public void SubmitScore(int playScore)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "Top Players",
                    Value = playScore,
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, PlayerFabUpdateStatsResult, PlayerFabUpdateStatsError);
    }

    void PlayerFabUpdateStatsResult(UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
    {
        Debug.Log("Successfully updated player stats");
    }

    void PlayerFabUpdateStatsError(PlayFabError updatePlayerStatisticsError)
    {
        Debug.Log("Failed to updated player stats - " + updatePlayerStatisticsError.ErrorMessage);
    }

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest()
        {
            StatisticName = "Top Players",
            StartPosition = 0,
            MaxResultsCount = 5,
        };

        PlayFabClientAPI.GetLeaderboard(request, PlayFabGetLeaderboardResult, PlayFabGetLeaderboardError);
    }

    void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult)
    {
        Debug.Log("Successfully retrieved leaderboard");
        leaderboardPopUp.UpdateUI(getLeaderboardResult.Leaderboard);
    }

    void PlayFabGetLeaderboardError(PlayFabError getLeaderboardError)
    {
        Debug.Log("Failed to retrieve leaderboard - " + getLeaderboardError.ErrorMessage);
    }
}
