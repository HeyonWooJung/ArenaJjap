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

    public GameObject acceptPanel;
    public Button acceptButton; // 수락 버튼
    public Image acceptTimer;

    public TextMeshProUGUI timerText; // 타이머 표시

    private int acceptCount; // 수락한 플레이어 수
    private int playerCount;
    private bool isMatching = false;
    private float elapsedTime = 0f;
    Coroutine timerCoroutine;

    public TMP_Text nickName;
    public TMP_Text nickName1;

    float duration = 15f; // 30초 동안 감소
    void Start()
    {
        acceptCount = 0;
        playerCount = 0;
        PhotonNetwork.JoinLobby();
        nickName.text = PhotonNetwork.NickName;
        nickName1.text = PhotonNetwork.NickName;

        acceptPanel.SetActive(false);
        matchPanel.gameObject.SetActive(false);

        timerText.text = "0:00";
    }
    public void StartMatchmaking()
    {
        matchPanel.gameObject.SetActive(true);
        matchButton.interactable = false;
        elapsedTime = 0f;
        timerCoroutine = StartCoroutine(TimerCountUp());

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount == 4)
        {
            photonView.RPC("EnableAcceptButton", RpcTarget.All);
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
            timerText.text = $"{minutes}:{seconds:D2}"; // "0:01", "0:02" 형태로 표시
            yield return null;
        }
    }


    [PunRPC]
    public void EnableAcceptButton()
    {
        acceptPanel.SetActive(true);
        acceptTimer.gameObject.SetActive(true);
        acceptButton.interactable = true;
        
        acceptTimer.fillAmount = 1;
        StartCoroutine(TimerImageFill());

        isMatching = false;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    IEnumerator TimerImageFill()
    {
        while (elapsedTime < duration)
        {
            acceptTimer.fillAmount = 1 - (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        acceptTimer.fillAmount = 0; // 최종적으로 0으로 설정
        if (acceptTimer.fillAmount <= 0)
        {
            yield return new WaitForSeconds(3);
            CancelMatch();
        }
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

        if (acceptCount == 4 && PhotonNetwork.IsMasterClient)
        {
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
        playerCount--;
        acceptCount--;
        isMatching = false;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        if (acceptPanel != null)
        {
            acceptPanel.SetActive(false);
        }

        PhotonNetwork.LeaveRoom();
        matchPanel.gameObject.SetActive(false);
        matchButton.interactable = true;
        timerText.text = "0:00";
    }

}
