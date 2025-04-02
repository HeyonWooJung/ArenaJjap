using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class InGameManager : MonoBehaviourPunCallbacks
{
    PlayerController target;
    public Image fillImage;
    public Vector3 offset = new Vector3(0, 0, 0);

    public Transform[] spawnPoints; // 인원 수 만큼 위치 설정
    SkillCooldownUI skillUI;
    // 인덱스에 대응하는 프리팹 이름 리스트
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
    [PunRPC]
    public void HPBar()
    {
        float ratio = Mathf.Clamp01(target.character.CurHP / target.character.HP);
        fillImage.fillAmount = ratio;

        transform.position = target.transform.position + offset;
        transform.forward = Camera.main.transform.forward;
    }
    void SpawnMyChampion()
    {
        int champIndex = MatchManager.myChampionIndex;

        if (champIndex < 0 || champIndex >= championNames.Length)
        {
            Debug.LogError("챔피언 인덱스 오류");
            return;
        }

        string prefabName = championNames[champIndex];
        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints[actorIndex].position;

        GameObject champObj = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        PlayerController player = champObj.GetComponent<PlayerController>();

        // 태그 설정
        PhotonView view = player.GetComponent<PhotonView>();
        if (actorIndex == 0 || actorIndex == 1)
        {
            view.RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 1);
        }
        else if (actorIndex == 2 || actorIndex == 3)
        {
            view.RPC("SetMyTag", RpcTarget.AllBufferedViaServer, 2);
        }

        // 내 플레이어인 경우만 UI 보여주고 연결
        if (view.IsMine)
        {
            foreach (var ui in canvasUIs)
                ui.SetActive(false);

            GameObject myUI = canvasUIs[champIndex];
            myUI.SetActive(true);

            // 자식들 중에서 SkillCooldownUI 찾기 (비활성화 포함)
            SkillCooldownUI uiScript = myUI.GetComponentInChildren<SkillCooldownUI>(true);
            if (uiScript != null)
            {
                // 부모 중 PlayerController가 붙은 오브젝트 찾아서 targetController에 연결
                Transform current = uiScript.transform;
                PlayerController controller = null;

                while (current != null)
                {
                    controller = current.GetComponent<PlayerController>();
                    if (controller != null)
                        break;

                    current = current.parent;
                }

                if (controller != null)
                {
                    uiScript.targetController = controller;
                }
            }
        }
        
    }
}