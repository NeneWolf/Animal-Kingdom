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
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                restartButton.SetActive(true);
            }
            else
            {
                restartButton.SetActive(false);
            }

            if (winner == PhotonNetwork.LocalPlayer)
            {
                UpdateWinnerText();
            }
            else if (winner != PhotonNetwork.LocalPlayer && winner != null)
            {
                UpdateLoserText();
            }
            else if(winner == null)
            {
                UpdateDrawText();
            }
        }
        else return;
    }

    private void UpdateWinnerText()
    {
        if(winner != null)
        {
            loserText.SetActive(false);
            string winnerMessage = "Winner: ";

            winnerMessage += winner + ", ";

            winnerText.text = winnerMessage.TrimEnd(',', ' ');
        }
    }

    private void UpdateLoserText()
    {
        if (winner != null)
        {
            loserText.SetActive(true);
            string winnerMessage = "Winner: ";

            winnerMessage += winner + ", ";

            winnerText.text = winnerMessage.TrimEnd(',', ' ');
        }
    }

    private void UpdateDrawText()
    {
        loserText.SetActive(false);
        string winnerMessage = "No Winners!";

        //winnerMessage += winner.NickName + " | " + secondWinner.NickName;

        winnerText.text = winnerMessage;
    }

    public void RestartGame()
    {
        multiplayerLevelManager.isRestarting = true;
        multiplayerLevelManager.RestartGame();
    }

}
