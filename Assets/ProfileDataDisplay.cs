using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileDataDisplay : MonoBehaviour
{
    [SerializeField] GameObject NoAvailableRecordTextG;
    [SerializeField] GameObject[] OtherData;

    [SerializeField] Text fieldName;
    [SerializeField] Text fieldBestScore;
    [SerializeField] Text fieldDate;
    [SerializeField] Text fieldTotalPlayers;
    [SerializeField] Text fieldRoomName;

    public void LoadProfileData()
    {
        PlayerData playerData = DataGameManager.instance.playerData;

        if(playerData.username != null)
        {
            NoAvailableRecordTextG.gameObject.SetActive(false);
            foreach (GameObject g in OtherData)
            {
                g.gameObject.SetActive(true);
            }
            fieldName.text = playerData.username;
            fieldBestScore.text = playerData.bestScore.ToString();
            fieldDate.text = playerData.bestScoreDate;
            fieldTotalPlayers.text = playerData.totalPlayersInGame.ToString();
            fieldRoomName.text = playerData.roomName;
        }
        else
        {
            NoAvailableRecordTextG.gameObject.SetActive(true);
            foreach (GameObject g in OtherData)
            {
                g.gameObject.SetActive(false);
            }
        }
    }

}
