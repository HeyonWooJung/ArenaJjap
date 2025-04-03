using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class InGameManager : MonoBehaviourPunCallbacks
{
    GameObject barPrefab;
    PlayerController myPlayer;

    public Transform[] spawnPoints; // 인원 수 만큼 위치 설정
    [SerializeField] private GameObject[] canvasUIs;

    private readonly string[] championNames = { "Ryze", "Sion", "Tryn", "Vayne" };

    void Start()
    {
        barPrefab = Resources.Load<GameObject>("HealthBarCanvas");

        SpawnMyChampion();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.inGameManager = this;
        }

        StartCoroutine(SetupAllHealthBars());
    }

    void SpawnMyChampion()
    {
        int champIndex = MatchManager.myChampionIndex;
        string prefabName = championNames[champIndex];

        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints[actorIndex].position;

        GameObject champObj = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        myPlayer = champObj.GetComponent<PlayerController>();
        PhotonView view = myPlayer.GetComponent<PhotonView>();

        // UI 활성화
        if (champIndex >= 0 && champIndex < canvasUIs.Length)
        {
            canvasUIs[champIndex].SetActive(true);
        }

        // 태그 설정
        if (actorIndex == 0 || actorIndex == 1)
        {
            view.RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 1); // 블루팀
        }
        else if (actorIndex == 2 || actorIndex == 3)
        {
            view.RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 2); // 레드팀
        }
    }

    IEnumerator SetupAllHealthBars()
    {
        yield return new WaitForSeconds(1f); // 모든 캐릭터가 생성될 때까지 잠깐 대기

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

        foreach (var player in allPlayers)
        {
            // 중복 생성 방지
            if (player.GetComponentInChildren<HealthBar>() != null)
                continue;

            GameObject bar = Instantiate(barPrefab);
            HealthBar healthBar = bar.GetComponent<HealthBar>();
            PhotonView view = player.GetComponent<PhotonView>();

            if (healthBar != null)
            {
                healthBar.target = player;
                healthBar.useLocalData = view.IsMine;
            }

            // 동기화 컴포넌트도 같이 활성화됨
            bar.GetComponent<HealthBarNetworkSync>();
        }
    }
}
