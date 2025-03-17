using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{    
    public TMP_Text nickName;
    public Text connectedUser;
    void Start()
    {
        PhotonNetwork.JoinLobby();
        if(AuthManager.user != null)
        {
            nickName.text = AuthManager.user.DisplayName;
        }
        else
        {
            nickName.text = "º“»ØªÁ";
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }
}
