using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TryndamereController : PlayerController
{
    private bool isMoving = false;

    private Animator anim;
    private int rage = 0; //  분노 시스템 추가

    public override void Move(Vector3 pos)
    {
        base.Move(pos);
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  애니메이터 가져오기 (필요할 때만)
        anim.SetFloat("isMovingBlend", 1f); //  이동 애니메이션 적용

        isMoving = true;
        StartCoroutine(CheckIfStopped());
    }

    private IEnumerator CheckIfStopped()
    {
        yield return new WaitForSeconds(0.1f); // 이동이 시작된 후 잠시 대기

        Vector3 lastPosition = transform.position;

        while (isMoving)
        {
            yield return new WaitForSeconds(0.1f); //  일정 시간마다 이동 여부 확인

            //  캐릭터의 위치가 변하지 않으면 이동 종료 (즉, Idle 상태로 전환)
            if (Vector3.Distance(lastPosition, transform.position) < 0.01f)
            {
                anim.SetFloat("isMovingBlend", 0f); //  Idle 애니메이션 실행
                isMoving = false;
                yield break;
            }

            lastPosition = transform.position; //  이전 위치 업데이트
        }
    }


    public override void AutoAttack(Character target)
    {
        base.AutoAttack(target);

        Debug.Log($" AutoAttack 실행! 대상: {target?.name}");

        GainRage(); //  분노 증가
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("Attack"); //  공격 애니메이션 실행
    }


    public override void SkillQ(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (rage > 0)
        {
            int healAmount = rage / 2;
            character.Heal(healAmount);
            rage = 0;
            Debug.Log($"Q 스킬 사용! 체력 {healAmount} 회복");
        }
        else
        {
            Debug.Log("Q 스킬 사용 불가: 분노 부족");
        }
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("UseQ");
    }

    public override void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        Debug.Log("W 스킬 사용! 적 공격력 감소");
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("UseW");
    }

    public override void SkillE(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        Debug.Log($"E 스킬 사용! {location} 방향으로 돌진");
        transform.position = Vector3.MoveTowards(transform.position, location, 5f); //  간단한 이동
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("UseE");
    }

    public override void SkillR(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        Debug.Log("R 스킬 사용! 5초간 무적");
        StartCoroutine(BecomeImmortal());
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("UseR");
    }

    private void GainRage()
    {
        rage += 5;
        Debug.Log($" 분노 증가: {rage}");
    }

    private IEnumerator BecomeImmortal()
    {
        character.SetState(State.Invincible);
        yield return new WaitForSeconds(5);
        character.SetState(State.Neutral);
        Debug.Log("R 스킬 효과 종료!");
    }
}




//using UnityEngine;

//public class TryndamereController : MonoBehaviour
//{
//    private Animator anim; // 애니메이터 컴포넌트
//    private bool isMoving; // 이동 여부
//    private float attackCooldown = 0.5f; // 공격 간 딜레이
//    private float lastAttackTime; // 마지막 공격 시간
//    private Vector3 targetPosition; // 이동 목표 위치
//    public float moveSpeed = 5f; // 이동 속도
//    public CharacterInfo character;

//    void Start()
//    {
//        anim = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        TrynMovement(); // 이동 처리
//        TrynAttack(); // 공격 처리
//        MoveCharacter(); // 실제 이동 적용
//        TrynSkills(); // QWER 스킬 실행 
//    }

//    void TrynMovement()
//    {
//        if (Input.GetMouseButtonDown(1)) // 마우스 우클릭 감지
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;

//            // Raycast를 사용하여 클릭한 위치 찾기
//            if (Physics.Raycast(ray, out hit))
//            {
//                targetPosition = hit.point; // 목표 위치 설정
//                isMoving = true;
//                anim.SetFloat("isMovingBlend", 1f); // 블렌드 트리 값 변경 (이동 애니메이션 실행)
//            }
//        }
//        if (Input.GetKeyDown(KeyCode.S)) //  S 키를 누르면 즉시 정지
//        {
//            isMoving = false;
//            targetPosition = transform.position; // 현재 위치를 목표로 설정하여 이동 중단
//            anim.SetFloat("isMovingBlend", 0f); // Idle 애니메이션으로 변경
//            Debug.Log("S 키 입력: 트린다미어 정지");
//            return;
//        }
//    }

//    void MoveCharacter()
//    {
//        if (isMoving)
//        {
//            // 목표 위치를 향하는 방향 계산
//            Vector3 direction = (targetPosition - transform.position).normalized;

//            // 캐릭터가 해당 방향을 바라보도록 회전
//            if (direction != Vector3.zero)
//            {
//                anim.SetTrigger("Idle");
//                Quaternion targetRotation = Quaternion.LookRotation(direction);
//                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
//            }

//            // 현재 위치에서 목표 위치로 이동
//            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

//            // 애니메이션 블렌드트리 값 부드럽게 증가
//            float blendValue = Mathf.Lerp(anim.GetFloat("isMovingBlend"), 1f, Time.deltaTime * 5f);
//            anim.SetFloat("isMovingBlend", blendValue);

//            // 목표 위치에 도달하면 이동 종료
//            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
//            {
//                isMoving = false;
//            }
//        }
//        else
//        {
//            // 이동이 끝나면 애니메이션 값을 부드럽게 감소 (Idle로 전환)
//            float blendValue = Mathf.Lerp(anim.GetFloat("isMovingBlend"), 0f, Time.deltaTime * 5f);
//            anim.SetFloat("isMovingBlend", blendValue);
//        }
//    }

//    void TrynAttack()
//    {
//        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown) // 공격 쿨타임 체크
//        {
//            anim.SetTrigger("Attack"); // 공격 애니메이션 실행
//            lastAttackTime = Time.time; // 마지막 공격 시간 업데이트
//        }
//    }

//    void TrynSkills()
//    {
//        if (Input.GetKeyDown(KeyCode.Q)) // Q 스킬 입력
//        {
//            anim.SetTrigger("UseQ");
//        }
//        if (Input.GetKeyDown(KeyCode.W)) // W 스킬 입력
//        {
//            anim.SetTrigger("UseW");
//        }
//        if (Input.GetKeyDown(KeyCode.E)) // E 스킬 입력
//        {
//            anim.SetTrigger("UseE");
//        }
//        if (Input.GetKeyDown(KeyCode.R)) // R 스킬 입력
//        {
//            anim.SetTrigger("UseR");
//        }
//    }
//}
