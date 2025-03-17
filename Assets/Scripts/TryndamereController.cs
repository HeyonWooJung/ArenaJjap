using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryndamereController : MonoBehaviour
{
    private Animator anim; // 애니메이터 컴포넌트
    private bool isMoving; // 이동 여부
    private float attackCooldown = 0.5f; // 공격 간 딜레이
    private float lastAttackTime; // 마지막 공격 시간
    private Vector3 targetPosition; // 이동 목표 위치
    public float moveSpeed = 5f; // 이동 속도

    void Start()
    {
        anim = GetComponent<Animator>(); // Animator 가져오기
    }

    void Update()
    {
        TrynMovement(); // 이동 처리
        TrynAttack(); // 공격 처리
        MoveCharacter(); // 실제 이동 적용
        TrynSkills(); // QWER 스킬 실행 
    }

    void TrynMovement()
    {
        if (Input.GetMouseButtonDown(1)) // 마우스 우클릭 감지
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast를 사용하여 클릭한 위치 찾기
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point; // 목표 위치 설정
                isMoving = true;
                anim.SetFloat("isMovingBlend", 1f); // 블렌드 트리 값 변경 (이동 애니메이션 실행)
            }
        }
    }

    void MoveCharacter()
    {
        if (isMoving)
        {
            // 목표 위치를 향하는 방향 계산
            Vector3 direction = (targetPosition - transform.position).normalized;

            // 캐릭터가 해당 방향을 바라보도록 회전
            if (direction != Vector3.zero)
            {
                anim.SetTrigger("Idle");
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // 현재 위치에서 목표 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 애니메이션 블렌드트리 값 부드럽게 증가
            float blendValue = Mathf.Lerp(anim.GetFloat("isMovingBlend"), 1f, Time.deltaTime * 5f);
            anim.SetFloat("isMovingBlend", blendValue);

            // 목표 위치에 도달하면 이동 종료
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
        else
        {
            // 이동이 끝나면 애니메이션 값을 부드럽게 감소 (Idle로 전환)
            float blendValue = Mathf.Lerp(anim.GetFloat("isMovingBlend"), 0f, Time.deltaTime * 5f);
            anim.SetFloat("isMovingBlend", blendValue);
        }
    }

    void TrynAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown) // 공격 쿨타임 체크
        {
            anim.SetTrigger("Attack"); // 공격 애니메이션 실행
            lastAttackTime = Time.time; // 마지막 공격 시간 업데이트
        }
    }

    void TrynSkills()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q 스킬 입력
        {
            anim.SetTrigger("UseQ");
        }
        if (Input.GetKeyDown(KeyCode.W)) // W 스킬 입력
        {
            anim.SetTrigger("UseW");
        }
        if (Input.GetKeyDown(KeyCode.E)) // E 스킬 입력
        {
            anim.SetTrigger("UseE");
        }
        if (Input.GetKeyDown(KeyCode.R)) // R 스킬 입력
        {
            anim.SetTrigger("UseR");
        }
    }
}
