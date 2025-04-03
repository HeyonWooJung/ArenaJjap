using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AutoConnectSolo : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("SoloRoom", new RoomOptions
        {
            MaxPlayers = 1,
            IsVisible = false,
            IsOpen = false
        }, TypedLobby.Default);
    }
}
