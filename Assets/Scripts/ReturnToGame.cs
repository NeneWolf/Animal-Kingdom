using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReturnToGame : MonoBehaviour, IPunObservable
{
    PhotonView photonView;

    [SerializeField] GameObject[] wolfsOnLoading;
    [SerializeField] TextMeshProUGUI loadingText;

    bool isLoadedCompleted;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if(photonView.IsMine && PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(ReturnToGameCount());
        }


        if (isLoadedCompleted)
        {
            loadingText.text = "Match is restarting...";
        }
    }

    IEnumerator ReturnToGameCount()
    {
        yield return new WaitForSeconds(5f);

        foreach(GameObject go in wolfsOnLoading)
        {
            go.GetComponent<Animator>().applyRootMotion = true;
        }

        isLoadedCompleted = true;
        yield return new WaitForSeconds(2f);

        PhotonNetwork.LoadLevel("GameScene_Multiplayer");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(isLoadedCompleted);
        }
        else if(stream.IsReading)
        {
            isLoadedCompleted = (bool)stream.ReceiveNext();
        }
    }
}
