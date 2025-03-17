using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ServerConnecter : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        // �� �� �ѱ�� �ٰ��� 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToServer() //��ư�� �����Ű���� �츮�� ���� ���� �޼���
    {
        PhotonNetwork.ConnectUsingSettings(); //���� ���� ���� ������ ������ ���� �����϶�� ����
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("���� ����");

        if (AuthManager.user != null)
        {
            PhotonNetwork.NickName = AuthManager.user.DisplayName;
            Debug.Log("�г��� ����");
        }
        else
        {
            PhotonNetwork.NickName = "";
        }
        //�� �ε��� �� ���� �ɸ��� , �κ� ������ �� �����ɸ� �� ������ �� �����ϴ�.
    }

}