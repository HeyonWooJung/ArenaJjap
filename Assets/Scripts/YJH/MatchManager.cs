using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MatchManager : MonoBehaviourPunCallbacks
{

    [SerializeField, Header("프로필 이미지")]
    public Image[] ProfileImgs;

    [SerializeField, Header("뒷 사진")]
    public GameObject[] BackgroundImgs;

    [SerializeField, Header("챔피언 버튼")]
    public Button[] championBtns;

    [Header("소환사들")]
    public Image[] summoners;

    [Header("닉네임 UI")]
    public TextMeshProUGUI[] nickNameTexts;


    private int currentIndex = -1;
    private List<Player> playerList = new List<Player>();    
    private List<Player> blueteam = new List<Player>();
    private List<Player> redteam = new List<Player>();

    private Dictionary<Player, int> playerChampion = new Dictionary<Player, int>();
    
    

    private void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AssignTeams();
        }
    }

    void AssignTeams()
    {
        playerList = new List<Player>(PhotonNetwork.PlayerList);

        blueteam.Add(playerList[0]);
        blueteam.Add(playerList[1]);
        redteam.Add(playerList[2]);
        redteam.Add(playerList[3]);

        photonView.RPC("SyncTeams", RpcTarget.All, playerList[0].ActorNumber, playerList[1].ActorNumber,
                                                 playerList[2].ActorNumber, playerList[3].ActorNumber);
    }

    [PunRPC]
    private void SyncTeams(int p1, int p2, int p3, int p4)
    {
        Player[] players = PhotonNetwork.PlayerList;

        blueteam.Clear();
        redteam.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == p1 || p.ActorNumber == p2)
                blueteam.Add(p);
            else if (p.ActorNumber == p3 || p.ActorNumber == p4)
                redteam.Add(p);            
        }

        for(int i=0; i< players.Length; i++)
        {
            nickNameTexts[i].text = players[i].NickName;
        }
    }

    [PunRPC] public void StartGame() { }

    private void Start()
    {
        foreach (var bg in BackgroundImgs)
        {
            bg.SetActive(false);
        }

        for (int i = 0; i < championBtns.Length; i++)
        {
            if (i == 4)
            {
                championBtns[i].onClick.AddListener(OnRandomChampionClick);
            }
            else
            {
                int index = i;
                championBtns[i].onClick.AddListener(() => OnChampionClick(index));
            }            
        }

    }

    private void OnChampionClick(int index)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        List<Player> myTeam = blueteam.Contains(localPlayer) ? blueteam : redteam;

        // 이전 선택한 챔피언 해제
        if (currentIndex != -1)
        {
            championBtns[currentIndex].interactable = true;
            photonView.RPC("HideBackgroundImage", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, currentIndex);
        }

        // 선택한 챔피언 적용
        championBtns[index].interactable = false;
        playerChampion[localPlayer] = index;


        // 모든 클라이언트에게 동기화
        photonView.RPC("SyncChampionSelection", RpcTarget.All, localPlayer.ActorNumber, index);

        // 현재 선택된 인덱스 업데이트
        currentIndex = index;
    }

    // 랜덤 버튼 클릭 시 실행
    private void OnRandomChampionClick()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        List<Player> myTeam = blueteam.Contains(localPlayer) ? blueteam : redteam;

        // 사용 가능한 챔피언 찾기
        List<int> availableChampions = new List<int> { 0, 1, 2, 3 };
        foreach (var member in myTeam)
        {
            if (playerChampion.ContainsKey(member))
            {
                availableChampions.Remove(playerChampion[member]);
            }
        }

        int randomIndex = availableChampions[Random.Range(0, availableChampions.Count)];
        OnChampionClick(randomIndex);
    }

    // 모든 클라이언트에서 챔피언 선택 정보 동기화
    [PunRPC]
    private void SyncChampionSelection(int actorNumber, int championIndex)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == actorNumber)
            {
                playerChampion[p] = championIndex;

                // **본인에게만 배경 이미지 표시**
                if (p == PhotonNetwork.LocalPlayer)
                {
                    BackgroundImgs[championIndex].SetActive(true);
                }
                // 같은 팀원도 챔피언 선택 반영 (버튼 비활성화)
                List<Player> myTeam = blueteam.Contains(p) ? blueteam : redteam;

                return;
            }
        }
    }

    // 배경 이미지 숨기기 (본인이 선택 변경 시)
    [PunRPC]
    private void HideBackgroundImage(int actorNumber, int championIndex)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            BackgroundImgs[championIndex].SetActive(false);
        }
    }

    

}
