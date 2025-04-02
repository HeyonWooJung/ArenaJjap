using UnityEngine;
using Photon.Pun;

public class ClickDebugger : MonoBehaviour
{
    public string enemyTag = "Enemy";

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);
                Debug.Log($"[디버그] 우클릭 대상: {hit.transform.name}");

                float distance = Vector3.Distance(hit.point, transform.position);
                PhotonView pv = hit.transform.GetComponent<PhotonView>();

                if (hit.transform.CompareTag(enemyTag))
                {
                    Debug.Log($"[적 인식] 거리: {distance}, PhotonView 있음: {pv != null}");
                }
                else
                {
                    Debug.Log("[적 아님] 이동 지점으로 간주됨");
                }
            }
        }
    }
}
