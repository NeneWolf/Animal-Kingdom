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

    public void LoadProfileData(PlayerData playerData)
    {
        //PlayerData playerData = DataGameManager.instance.playerData;

        if(playerData.bestScore > 0)
        {
            NoAvailableRecordTextG.gameObject.SetActive(false);
            print("here");
            print(playerData.bestScore);

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
            print("Should be here");
            foreach (GameObject g in OtherData)
            {
                g.gameObject.SetActive(false);
            }
        }
    }

}
