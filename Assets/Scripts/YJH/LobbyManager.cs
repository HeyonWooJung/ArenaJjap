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
    public Transform lobbyUser;
    public Text connectedUser;
    void Start()
    {
        PhotonNetwork.JoinLobby();        
        nickName.text = PhotonNetwork.NickName;
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        foreach(TypedLobby lobby in lobbyStatistics)
        {
            Debug.Log("³ª¿È");
            var userName = Instantiate(connectedUser, lobbyUser);
            userName.text = nickName.text;
        }
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }
}
