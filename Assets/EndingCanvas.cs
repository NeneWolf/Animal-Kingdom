using Photon.Pun;
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

    private List<Player> winners = new List<Player>();

    MultiplayerLevelManager multiplayerLevelManager;
    PhotonView photonView;

    private void Awake()
    {
        multiplayerLevelManager = GameObject.FindAnyObjectByType<MultiplayerLevelManager>().GetComponent<MultiplayerLevelManager>();
    }

    public void SendInformationToEndingCanvas(GameObject player)
    {
        photonView = player.GetComponent<PhotonView>();
    }

    public void UpdateInformation(List<Player> winners)
    {
        this.winners = winners;
        panel.SetActive(true);

        if(photonView.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                restartButton.SetActive(true);
            }
            else
            {
                restartButton.SetActive(false);
            }


            if (winners.Contains(PhotonNetwork.LocalPlayer))
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
        string winnerMessage = "Winners: ";

        foreach (var winner in winners)
        {
            winnerMessage += winner.NickName + ", ";
        }

        winnerText.text = winnerMessage.TrimEnd(',', ' ');
    }

    private void UpdateLoserText()
    {
        loserText.SetActive(true);
        string winnerMessage = "Winners: ";

        foreach (var winner in winners)
        {
            winnerMessage += winner.NickName + ", ";
        }

        winnerText.text = winnerMessage.TrimEnd(',', ' ');
    }

    public void RestartGame()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("GameScene_Multiplayer");
    }
}
