using UnityEngine;
using Photon.Pun;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // �ο� �� ��ŭ ��ġ ����

    // �ε����� �����ϴ� ������ �̸� ����Ʈ
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
