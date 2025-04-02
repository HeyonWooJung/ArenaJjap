using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class InGameManager : MonoBehaviourPunCallbacks
{
    PlayerController target;

    public Transform[] spawnPoints; // 인원 수 만큼 위치 설정
    SkillCooldownUI skillUI;

    private readonly string[] championNames = { "Ryze", "Sion", "Tryn", "Vayne" };

    [SerializeField] private GameObject[] canvasUIs;


    void Start()
    {
        SpawnMyChampion();
        if(GameManager.Instance != null)
        {
            GameManager.Instance.inGameManager = this;
        }
    }
    void SpawnMyChampion()
    {
        int champIndex = MatchManager.myChampionIndex;

        string prefabName = championNames[champIndex];
        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        Vector3 spawnPos = spawnPoints[actorIndex].position;

        GameObject champObj = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        PlayerController player = champObj.GetComponent<PlayerController>();
        PhotonView view = player.GetComponent<PhotonView>();

        // 태그 설정
        if (actorIndex == 0 || actorIndex == 1)
        {
            view.RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 1);
        }
        else if (actorIndex == 2 || actorIndex == 3)
        {
            view.RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 2);
        }
        if (view.IsMine)
        {
            GameObject barPrefab = Resources.Load<GameObject>("HealthBarCanvas");
            if (barPrefab != null)
            {
                GameObject bar = Instantiate(barPrefab);
                HealthBar healthBar = bar.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.target = player;
                    healthBar.useLocalData = true;
                }

                HealthBarNetworkSync sync = bar.GetComponent<HealthBarNetworkSync>();
                if (sync != null)
                {
                    sync.photonView.ViewID = view.ViewID; // 연결된 캐릭터와 같은 ViewID 사용
                }
            }
            else
            {
                Debug.LogError("HealthBarCanvas 프리팹을 Resources/UI 안에서 찾을 수 없습니다.");
            }
        }
        else
        {
            // 내 챔피언이 아니더라도 체력바 생성 필요
            GameObject barPrefab = Resources.Load<GameObject>("HealthBarCanvas");
            if (barPrefab != null)
            {
                GameObject bar = Instantiate(barPrefab);
                HealthBar healthBar = bar.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.target = player;
                    healthBar.useLocalData = false; // RPC 동기화용
                }

                HealthBarNetworkSync sync = bar.GetComponent<HealthBarNetworkSync>();
                if (sync != null)
                {
                    sync.photonView.ViewID = view.ViewID;
                }
            }
        }
    }    
}