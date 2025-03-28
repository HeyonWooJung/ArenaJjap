using UnityEngine;
using Photon.Pun;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // 인원 수 만큼 위치 설정

    // 인덱스에 대응하는 프리팹 이름 리스트
    private readonly string[] championNames = { "Ryze", "Sion1", "Tryn1", "Vayne1" };

    void Start()
    {
        SpawnMyChampion();
    }


    void SpawnMyChampion()
    {
        int champIndex = MatchManager.myChampionIndex;

        if (champIndex < 0 || champIndex >= championNames.Length)
        {
            return;
        }

        string prefabName = championNames[champIndex];

        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints[actorIndex].position;
        PlayerController player = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity).GetComponent<PlayerController>();
        if (actorIndex == 0 || actorIndex == 1)
        {
            player.GetComponent<PhotonView>().RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 1);
        }
        else if (actorIndex == 2 || actorIndex == 3)
        {
            player.GetComponent<PhotonView>().RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 2);
        }
    }
}