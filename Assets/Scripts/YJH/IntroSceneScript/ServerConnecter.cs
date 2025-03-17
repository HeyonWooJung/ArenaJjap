using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ServerConnecter : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        // ▼ 씬 넘기면 다같이 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToServer() //버튼에 연결시키고자 우리가 직접 만든 메서드
    {
        PhotonNetwork.ConnectUsingSettings(); //내가 가진 세팅 정보를 가지고 서버 연결하라고 지시
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결");

        if (AuthManager.user != null)
        {
            PhotonNetwork.NickName = AuthManager.user.DisplayName;
            Debug.Log("닉네임 설정");
        }
        else
        {
            PhotonNetwork.NickName = "";
        }
        //씬 로딩이 더 오래 걸릴지 , 로비 입장이 더 오래걸릴 지 보장할 수 없습니다.
    }

}