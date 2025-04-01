using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AutoStartGame : MonoBehaviourPunCallbacks
{

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // 마스터 서버 연결
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 마스터 서버에 연결됨!");
        PhotonNetwork.JoinLobby(); // 로비 입장
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료!");
        PhotonNetwork.JoinRandomRoom(); // 랜덤 방 입장 시도
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 방 실패 → 새 방 생성");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 1 }; // 1인 방
        PhotonNetwork.CreateRoom("SoloRoom", roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공!");

    }
}
