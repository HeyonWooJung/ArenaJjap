using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; //  따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 10, -5); // 카메라 오프셋 (조절 가능)
    public float smoothSpeed = 5f; //  부드러운 이동 속도

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset; // 목표 위치
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // 부드러운 이동
        transform.LookAt(target); //  항상 플레이어 바라보기
    }
}

