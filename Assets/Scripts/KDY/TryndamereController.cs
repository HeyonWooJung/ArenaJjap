using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TryndamereController : PlayerController
{
    public GameObject healEffectPrefab;
    private TryndamereSkills skills;
    private bool isMoving = false;

    private Animator anim;
    public int rage = 0; //  분노 시스템 추가
    public int maxRage = 100; // 분노 최대값 설정
    private float critChance = 0.25f; //  기본 치명타 확률 (25%)

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

        GainRage(false); //  분노 증가
        if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
        anim.SetTrigger("Attack"); //  공격 애니메이션 실행
    }

    // 트린다미어 Q스킬
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


    public override void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        float skillRange = 50.5f;
        float attackReductionPercent = 0.2f; // ✅ 공격력 감소율 (20%)
        float moveSlowAmount = 0.4f;
        float effectDuration = 4f; // ✅ 효과 지속 시간 (공격력 & 이속 복구 시간)

        Collider[] targets = Physics.OverlapSphere(transform.position, skillRange, LayerMask.GetMask("Enemy"));
        Debug.Log($"W 스킬 사용! 대상 검색 중... 감지된 개수: {targets.Length}");

        foreach (Collider targetCollider in targets)
        {

            PlayerController targetPlayer = targetCollider.GetComponent<PlayerController>();

            if (targetPlayer == null || targetPlayer == this) continue;

            // ✅ 공격력 감소 적용
            float attackReduction = targetPlayer.character.ATK * attackReductionPercent;
            targetPlayer.character.AdjustATK(-attackReduction);
            StartCoroutine(RestoreAttackPower(targetPlayer, attackReduction, effectDuration));

            // ✅ 적이 등을 돌린 경우 이동 속도 감소
            if (IsEnemyFacingAway(targetPlayer.transform))
                StartCoroutine(SlowCharacter(targetPlayer, moveSlowAmount, effectDuration));
                Debug.Log($"{targetPlayer.name} 이동 속도 감소 적용!");  
        }

        anim?.SetTrigger("UseW");
    }

    // ✅ 공격력 4초 후 복구
    private IEnumerator RestoreAttackPower(PlayerController target, float amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        target.character.AdjustATK(amount);
        Debug.Log($"{target.name} ATK 복구 완료! (+{amount})");
    }

    // ✅ 이동 속도 4초 후 복구
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

    // ✅ 적이 등을 돌렸는지 확인
    private bool IsEnemyFacingAway(Transform target)
    {
        Vector3 directionToEnemy = (transform.position - target.position).normalized;
        directionToEnemy.y = 0;
        return Vector3.Dot(target.forward, directionToEnemy) < 0;
    }







    //public override void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    //{
    //    Debug.Log("W 스킬 사용! 적 공격력 감소");
    //    if (anim == null) anim = GetComponentInChildren<Animator>(); //  필요할 때만 애니메이터 가져오기
    //    anim.SetTrigger("UseW");


    //}

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

    private void GainRage(bool isCritical)
    {
        int rageGain = isCritical ? 10 : 5; //  치명타 시 추가 분노 증가
        rage += rageGain;

        if (rage > maxRage)
        {
            rage = maxRage; //  최대값 초과 방지
        }
        Debug.Log($" 분노 증가: {rage}");
    }

    private IEnumerator BecomeImmortal()
    {
        character.SetState(State.Invincible);
        yield return new WaitForSeconds(5);
        character.SetState(State.Neutral);
        Debug.Log("R 스킬 효과 종료!");
    }

    public void PerformAttack()
    {
        bool isCritical = Random.value < critChance; //  치명타 확률 체크
        GainRage(isCritical); // 치명타 여부에 따라 분노 증가

        if (isCritical)
        {
            Debug.Log(" 치명타 발생!");
            anim.SetTrigger("CriticalHit"); //  치명타 애니메이션 실행
        }
        else
        {
            anim.SetTrigger("Attack"); //  일반 공격 애니메이션 실행
        }
    }

}