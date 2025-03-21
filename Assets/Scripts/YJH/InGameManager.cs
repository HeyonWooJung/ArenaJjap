using UnityEngine;
using Photon.Pun;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // 인원 수 만큼 위치 설정

    // 인덱스에 대응하는 프리팹 이름 리스트
    private readonly string[] championNames = { "Ryze1", "Sion1", "Tryn1", "Vayne1" };

    void Start()
    {
        SpawnMyChampion();
    }

    void SpawnMyChampion()
    {
        int champIndex = MatchManager.myChampionIndex;

        Debug.Log(champIndex);

        if (champIndex < 0 || champIndex >= championNames.Length)
        {
            return;
        }

        string prefabName = championNames[champIndex];

        Debug.Log(prefabName);

        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints[actorIndex % spawnPoints.Length].position;

        PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
    }
}
