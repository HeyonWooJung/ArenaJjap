using UnityEngine;

public class TryndamereAttackTester : MonoBehaviour
{
    public TryndamereController trynd;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PlayerController target = hit.collider.GetComponent<PlayerController>();
                if (target != null)
                {
                    trynd.AutoAttack(target);
                    Debug.Log($"공격 시도: {target.name}");
                }
            }
        }
    }
}
