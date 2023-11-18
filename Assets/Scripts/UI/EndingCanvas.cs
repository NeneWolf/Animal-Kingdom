using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCanvas : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text winnerText;
    [SerializeField] GameObject loserText;
    [SerializeField] GameObject restartButton;

    MultiplayerLevelManager multiplayerLevelManager;
    PhotonView photonView;

    Player winner;
    Player secondWinner;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        multiplayerLevelManager = GameObject.FindAnyObjectByType<MultiplayerLevelManager>().GetComponent<MultiplayerLevelManager>();
    }

    public void SendInformationToEndingCanvas(GameObject player)
    {
        photonView = player.GetComponent<PhotonView>();
    }

    public void UpdateInformation(Player winner)
    {
        this.winner = winner;

        panel.SetActive(true);

        if(photonView.IsMine)
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    restartButton.SetActive(true);
            //}
            //else
            //{
            //    restartButton.SetActive(false);
            //}

            if (winner == PhotonNetwork.LocalPlayer)
            {
                UpdateWinnerText();
            }
            else
            {
                UpdateLoserText();
            }
        }
        else return;
    }

    private void UpdateWinnerText()
    {
        loserText.SetActive(false);
        string winnerMessage = "Winner: ";

        winnerMessage += winner + ", ";

        winnerText.text = winnerMessage.TrimEnd(',', ' ');
    }

    private void UpdateLoserText()
    {
        loserText.SetActive(true);
        string winnerMessage = "Winner: ";

        winnerMessage += winner + ", ";

        winnerText.text = winnerMessage.TrimEnd(',', ' ');
    }

    //private void UpdateDrawText()
    //{
    //    loserText.SetActive(false);
    //    string winnerMessage = "Winners: ";

    //    winnerMessage += winner.NickName + " | " + secondWinner.NickName;

    //    winnerText.text = winnerMessage.TrimEnd(',', ' ');
    //}

    public void RestartGame()
    {
       
    }

}
