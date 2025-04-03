using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoundCheck : MonoBehaviour
{
    [SerializeField] Text fullRound;

    PhotonView pv;
    int round;

    public void Init(PhotonView photon, int rd)
    {
        pv = photon;
        round = rd;

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.RC = this;
        }
    }

    public void RoundChecking()
    {
        pv.RPC("ShowRound", RpcTarget.AllBuffered, GameManager.Instance.round);
    }

    [PunRPC]
    public void ShowRound(int rd)
    {        
        fullRound.text = $"{rd + 1} 라운드";
    }
}
