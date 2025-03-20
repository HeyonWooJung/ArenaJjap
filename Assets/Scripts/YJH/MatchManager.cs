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
    public GameObject championPanel;
    public Button readyBtn;

    [SerializeField, Header("������ �̹���")]
    public Image[] ProfileImgs;

    [SerializeField, Header("�� ����")]
    public GameObject[] BackgroundImgs;

    [SerializeField, Header("è�Ǿ� ��ư")]
    public Button[] championBtns;

    [Header("��ȯ���")]
    public Image[] summoners;

    [Header("�г��� UI")]
    public TextMeshProUGUI[] nickNameTexts;


    private int currentIndex = -1;
    private List<Player> playerList = new List<Player>();    
    private List<Player> blueteam = new List<Player>();
    private List<Player> redteam = new List<Player>();

    private Dictionary<Player, int> playerChampion = new Dictionary<Player, int>();
    private int readyCount = 0;
    

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

        if (currentIndex != -1)
        {
            championBtns[currentIndex].interactable = true;
            photonView.RPC("HideBackgroundImage", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, currentIndex);
        }

        // ������ è�Ǿ� ����
        championBtns[index].interactable = false;
        playerChampion[localPlayer] = index;

        
        photonView.RPC("SyncChampionSelection", RpcTarget.All, localPlayer.ActorNumber, index);

        // ���� ���õ� �ε��� ������Ʈ
        currentIndex = index;

        readyBtn.interactable = true;

    }

    private void OnRandomChampionClick()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        List<Player> myTeam = blueteam.Contains(localPlayer) ? blueteam : redteam;

        // ��� ������ è�Ǿ� ã��
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

    // ��� Ŭ���̾�Ʈ���� è�Ǿ� ���� ���� ����ȭ
    [PunRPC]
    private void SyncChampionSelection(int actorNumber, int championIndex)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == actorNumber)
            {
                playerChampion[p] = championIndex;

                if (p == PhotonNetwork.LocalPlayer)
                {
                    BackgroundImgs[championIndex].SetActive(true);
                }
                List<Player> myTeam = blueteam.Contains(p) ? blueteam : redteam;

                return;
            }
        }
    }

    // ��� �̹��� ����� (������ ���� ���� ��)
    [PunRPC]
    private void HideBackgroundImage(int actorNumber, int championIndex)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            BackgroundImgs[championIndex].SetActive(false);
        }
    }

    public void OnReadyClick()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        if (playerChampion.ContainsKey(localPlayer))
        {
            championPanel.SetActive(false);
            readyBtn.interactable = false; // �غ� ��ư ��Ȱ��ȭ
            photonView.RPC("PlayerReady", RpcTarget.All, localPlayer.ActorNumber, playerChampion[localPlayer]);
        }
    }

    [PunRPC]
    public void PickChampion(int actorNumber, int championIndex)
    {
        // �ش� è�Ǿ� ���� ��ư ��Ȱ��ȭ
        if (championIndex >= 0 && championIndex < championBtns.Length)
        {
            championBtns[championIndex].interactable = false;
        }

        readyCount++;

        // ��� �÷��̾ �غ� �Ϸ�Ǹ� Scene4�� �̵�
        if (readyCount == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }
    

}
