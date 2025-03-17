using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; //  ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0, 10, -5); // ī�޶� ������ (���� ����)
    public float smoothSpeed = 5f; //  �ε巯�� �̵� �ӵ�

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset; // ��ǥ ��ġ
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // �ε巯�� �̵�
        transform.LookAt(target); //  �׻� �÷��̾� �ٶ󺸱�
    }
}

