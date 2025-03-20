using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TryndamereController : PlayerController
{
    // 애니메이션 및 상태 관련 변수
    public GameObject healEffectPrefab; // Q 스킬 사용 시 힐 이펙트 프리팹
    public GameObject rEffectObject; // R스킬 사용 시 파티클 
    private Animator anim; // 애니메이터
    private bool isMoving = false; // 이동 상태 확인

    // 전투 관련 변수
    public int rage = 0; // 현재 분노 수치
    public int maxRage = 100; // 최대 분노 수치
    private float critChance = 0.25f; // 기본 치명타 확률 (25%)

    // E 스킬 관련 변수
    private bool isDashing = false; // 돌진 여부
    private Vector3 eTargetPosition; // 목표 위치 저장
    public float eSpeed = 10f; // 이동 속도

    // R 스킬 관련 변수
    private bool isImmortal = false; // 무적 상태 여부
    public float rDuration = 5f; // R 스킬 지속 시간

    // W 스킬 관련 변수
    float skillRange = 50.5f; // W 스킬 범위
    float attackReductionPercent = 0.2f; // 공격력 감소율 (20%)
    float moveSlowAmount = 0.4f; // 이동 속도 감소율
    float effectDuration = 4f; // 효과 지속 시간 (공격력 & 이동 속도 복구 시간)

    // 네비게이션 에이전트 (이동 관련)
    private NavMeshAgent navMeshAgent;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // 이동 처리 함수
    public override void Move(Vector3 pos)
    {
        base.Move(pos);
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  애니메이터 가져오기 (필요할 때만)
        anim.SetFloat("isMovingBlend", 1f); //  이동 애니메이션 적용

        isMoving = true;
        StartCoroutine(CheckIfStopped());
    }

    // 이동 멈춤 감지
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

    //기본 공격 실행
    public override void AutoAttack(Character target)
    {
        base.AutoAttack(target);

        Debug.Log($" AutoAttack 실행! 대상: {target?.name}");

        GainRage(false); //  분노 증가
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("Attack"); //  공격 애니메이션 실행
    }

    // 트린다미어 Q스킬 - 체력회복
    public override void SkillQ(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (rage > 0)
        {
            int healAmount = rage / 2;
            character.Heal(healAmount);
            rage = 0;
            Debug.Log($"Q 스킬 사용! 체력 {healAmount} 회복");

            if (healEffectPrefab != null)
            {
                GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
                healEffect.transform.SetParent(transform); //  캐릭터에 붙이기
                healEffect.SetActive(true); // 비활성화되어 있다면 활성화
                Destroy(healEffect, 2f); //  2초 후 삭제
            }
        }

        if (anim == null) anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("UseQ");
    }

    // 트린다미어 W스킬 - 적 공격력이랑 이속 감소
    public override void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {

        Collider[] targets = Physics.OverlapSphere(transform.position, skillRange, LayerMask.GetMask("Enemy"));
        Debug.Log($"W 스킬 사용! 대상 검색 중... 감지된 개수: {targets.Length}");

        foreach (Collider targetCollider in targets)
        {

            PlayerController targetPlayer = targetCollider.GetComponent<PlayerController>();

            if (targetPlayer == null || targetPlayer == this) continue;

            //  공격력 감소 적용
            float attackReduction = targetPlayer.character.ATK * attackReductionPercent;
            targetPlayer.character.AdjustATK(-attackReduction);
            StartCoroutine(RestoreAttackPower(targetPlayer, attackReduction, effectDuration));

            //  적이 등을 돌린 경우 이동 속도 감소
            if (IsEnemyFacingAway(targetPlayer.transform))
                StartCoroutine(SlowCharacter(targetPlayer, moveSlowAmount, effectDuration));
                Debug.Log($"{targetPlayer.name} 이동 속도 감소 적용!");  
        }

        anim?.SetTrigger("UseW");
    }

    //  공격력 4초 후 감소 해제
    private IEnumerator RestoreAttackPower(PlayerController target, float amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        target.character.AdjustATK(amount);
        Debug.Log($"{target.name} ATK 복구 완료! (+{amount})");
    }

    //  이동 속도 4초 후 감소 해제
    private IEnumerator SlowCharacter(PlayerController target, float slowAmount, float duration)
    {
        if (target == null) yield break;

        target.character.SetCanRush(false);
        int tempSpeed = (int)(target.character.MoveSpeed * slowAmount);
        target.character.AdjustMoveSpeed(-tempSpeed);
        Debug.Log($"{target.name} 이동 속도 감소 시작! (-{tempSpeed})");

        yield return new WaitForSeconds(duration);

        target.character.AdjustMoveSpeed(tempSpeed);
        target.character.SetCanRush(true);
        Debug.Log($"{target.name} 이동 속도 복구 완료!");
    }

    // 적이 등을 돌렸는지 확인
    private bool IsEnemyFacingAway(Transform target)
    {
        Vector3 directionToEnemy = (transform.position - target.position).normalized;
        directionToEnemy.y = 0;
        return Vector3.Dot(target.forward, directionToEnemy) < 0;
    }

    // 트린다미어 E 스킬 (돌진 공격)
    public override void SkillE(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (isDashing) return; // 이미 돌진 중이면 실행 안 함

        eTargetPosition = location; // 목표 위치 설정
        isDashing = true; // 돌진 시작

        // NavMeshAgent가 있다면 일시적으로 비활성화하여 충돌 방지
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.updatePosition = false;
            navMeshAgent.enabled = false;
        }

        if (anim == null) anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("UseE");

        StartCoroutine(DashMovement()); // 이동 처리 코루틴 실행
    }

    // E 스킬 이동 처리 (목표 지점까지 자연스럽게 이동)
    private IEnumerator DashMovement()
    {
        while (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, eTargetPosition, eSpeed * Time.deltaTime);

            // 목표 지점에 도달하면 돌진 종료
            if (Vector3.Distance(transform.position, eTargetPosition) < 0.1f)
            {
                isDashing = false;

                // NavMeshAgent를 다시 활성화하여 정상적인 이동 가능하도록 설정
                if (navMeshAgent != null)
                {
                    navMeshAgent.enabled = true;
                    navMeshAgent.Warp(transform.position);
                    navMeshAgent.isStopped = false;
                    navMeshAgent.updatePosition = true;
                }
            }
            yield return null;
        }
    }

    // R 스킬 (불사의 분노) - 일정 시간 동안 무적 상태 유지
    public override void SkillR(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (isImmortal) return; // 이미 무적이면 실행 안 함

        isImmortal = true; // 무적 상태 활성화
        character.SetState(State.Invincible); // 캐릭터 상태를 무적으로 설정

        if (anim == null) anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("UseR");

        // 파티클 이펙트 활성화
        if (rEffectObject != null)
        {
            rEffectObject.SetActive(true); // 궁극기 이펙트 활성화
        }

        StartCoroutine(DelayedEffectActivation());
        StartCoroutine(EndRSkill()); // 5초 후 무적 해제 및 이펙트 제거
    }

    // 2.5초 후 R 스킬 이펙트 활성화
    private IEnumerator DelayedEffectActivation()
    {
        yield return new WaitForSeconds(6f); // 2.5초 대기
        if (rEffectObject != null)
        {
            rEffectObject.SetActive(true); // 궁극기 이펙트 활성화
        }
    }

    private IEnumerator EndRSkill()
    {
        yield return new WaitForSeconds(5f); // 지속 시간 유지

        isImmortal = false; // 무적 상태 해제
        character.SetState(State.Neutral); // 원래 상태 복구

        // 파티클 이펙트 비활성화
        if (rEffectObject != null)
        {
            rEffectObject.SetActive(false); // 궁극기 이펙트 종료
        }
    }

    //// 일정 시간 후 R 스킬 효과 해제
    //private IEnumerator EndRSkill()
    //{
    //    yield return new WaitForSeconds(5f);
    //    isImmortal = false; // 무적 상태 해제
    //    character.SetState(State.Neutral); // 캐릭터 상태를 원래대로 복구
    //}

    // 피해 처리 (무적 상태일 경우 피해를 받지 않음)
    public void TakeDamage(float damage, bool isTrueDamage, float lethality, float armorPenetration)
    {
        if (isImmortal)
        {
            return; // 무적 상태일 경우 피해 무시
        }

        // 체력 감소 처리
        character.AdjustHP(-damage);

        // 무적 상태일 경우 최소 체력 1 유지
        if (isImmortal && character.CurHP <= 1)
        {
            character.AdjustHP(1 - character.CurHP);
        }
    }

    // 기본 공격 실행 (치명타 여부 확인 및 애니메이션 실행)
    public void PerformAttack()
    {
        bool isCritical = Random.value < critChance; // 치명타 확률 판정
        GainRage(isCritical); // 치명타 여부에 따라 분노 증가

        if (isCritical)
        {
            anim.SetTrigger("CriticalHit"); // 치명타 발생 시 애니메이션 실행
        }
        else
        {
            anim.SetTrigger("Attack"); // 일반 공격 애니메이션 실행
        }
    }

    // 분노 증가 처리 (치명타 발생 시 추가 증가)
    private void GainRage(bool isCritical)
    {
        int rageGain = isCritical ? 10 : 5; // 치명타 발생 시 +10, 일반 공격 시 +5
        rage += rageGain;

        if (rage > maxRage)
        {
            rage = maxRage; // 최대 분노 초과 방지
        }
    }
}
