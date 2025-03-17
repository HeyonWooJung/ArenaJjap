using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject matchPanel;
    public Button matchButton;

    public Button acceptButton; // ���� ��ư
    public TextMeshProUGUI timerText; // Ÿ�̸� ǥ��

    private int acceptCount = 0; // ������ �÷��̾� ��
    private bool isMatching = false;
    private float elapsedTime = 0f;
    Coroutine timerCoroutine;

    public TMP_Text nickName;
    public TMP_Text nickName1;
    void Start()
    {
        PhotonNetwork.JoinLobby();        
        nickName.text = PhotonNetwork.NickName;
        nickName1.text = PhotonNetwork.NickName;

        matchButton.onClick.AddListener(StartMatchmaking);
        acceptButton.onClick.AddListener(AcceptMatch);

        acceptButton.gameObject.SetActive(false);
        matchPanel.gameObject.SetActive(false);

        timerText.text = "0:00";
    }
    public void StartMatchmaking()
    {
        matchPanel.gameObject.SetActive(true);
        matchButton.interactable = false;
        elapsedTime = 0f;
        timerCoroutine = StartCoroutine(TimerCountUp());
        StartCoroutine(FindOrCreateRoom());
    }

    IEnumerator FindOrCreateRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        yield return new WaitForSeconds(2);

        if (!PhotonNetwork.InRoom) 
        {
            Debug.Log("�����");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
        }
    }

    IEnumerator TimerCountUp()
    {
        isMatching = true;
        while (isMatching)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = $"{minutes}:{seconds:D2}"; // "0:01", "0:02" ���·� ǥ��
            yield return null;
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.PlayerList.Length == 4)
        {
            photonView.RPC("EnableAcceptButton", RpcTarget.All);
        }
    }
     
    [PunRPC]
    public void EnableAcceptButton()
    {
        acceptButton.gameObject.SetActive(true);
        Debug.Log("���� ��� ��");
    }
    
    public void AcceptMatch()
    {
        photonView.RPC("PlayerAccepted", RpcTarget.All);
        acceptButton.interactable = false;
    }

    [PunRPC]
    public void PlayerAccepted()
    {
        acceptCount++;

        if (acceptCount == 4) // ��� �÷��̾ �����ϸ� ���� ����
        {
            isMatching = false; // Ÿ�̸� ����
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        PhotonNetwork.LoadLevel("Scene3"); 
    }

    public void CancelMatch()
    {
        isMatching = false; 
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        PhotonNetwork.LeaveRoom();
        matchPanel.gameObject.SetActive(false);
        matchButton.interactable = true;
        timerText.text = "0:00"; 
    }
}
