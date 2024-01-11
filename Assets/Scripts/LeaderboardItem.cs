using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    public Text order;
    public Text userName;
    public Text score;

    public void SetScores(int _order, string _userName, int _score)
    {
        order.text = _order.ToString();
        userName.text = _userName;
        score.text = _score.ToString();
    }
}
