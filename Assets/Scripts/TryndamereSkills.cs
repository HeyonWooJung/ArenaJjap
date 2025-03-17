using System.Collections;
using UnityEngine;

public class TryndamereSkills : MonoBehaviour
{

    public GameObject healEffectPrefab;
    private Animator anim;

    // 체력 및 분노 시스템
    public int maxHealth = 100; // 최대 체력
    public int currentHealth; // 현재 체력
    public int maxRage = 5000; // 최대 분노 100
    public int currentRage; // 현재 분노
    public int ragePerAttack = 5; // 기본 공격 시 분노 증가량
    public int ragePerCrit = 10; // 치명타 발생 시 추가 분노 증가량
    public float critChance = 0.25f; // 치명타 확률 (25%)


    //  E 스킬 관련 변수 추가
    public float maxEDistance = 7f; // E 스킬 최대 이동 거리
    public float eSpeed = 10f; // E 스킬 이동 속도
    private Vector3 eTargetPosition; // E 스킬 목표 위치
    private bool isDashing = false; // 돌진 중인지 여부

    // R 스킬 관련 변수
    private bool isImmortal = false; // 불멸 상태 여부

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth; // 시작 시 최대 체력
        currentRage = 5000; // 시작 시 분노 0
    }

    void Update()
    {
        HandleSkills();
        HandleDash();
    }

    void HandleSkills()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q 스킬
        {
            anim.SetTrigger("UseQ"); // Q 스킬 애니메이션 실행
            UseQSkill(); // Q 스킬 기능 실행
        }
        if (Input.GetKeyDown(KeyCode.W)) // W 스킬 
        {
            anim.SetTrigger("UseW"); // W 스킬 애니메이션 실행
            UseWSkill(); // W 스킬 기능 실행
        }
        if (Input.GetKeyDown(KeyCode.E)) //  E 스킬
        {
            StartDash();
        }
        if (Input.GetKeyDown(KeyCode.R)) //  R 스킬
        {
            anim.SetTrigger("UseR");
            UseRSkill();
        }
    }

    // Q 스킬 (피의 갈망) - 체력 회복
    void UseQSkill()
    {
        if (currentRage > 0) // 분노가 있을 때만 사용 가능
        {
            int healAmount = currentRage / 2; // 분노량의 절반만큼 체력 회복
            Heal(healAmount); // 체력 회복 적용
            currentRage = 0; // 분노 초기화
            anim.SetTrigger("UseQ");

            //  힐 이펙트 생성
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(transform); // 캐릭터를 부모로 설정하여 함께 이동하도록 함
            Destroy(healEffect, 2f); // 2초 후 자동 삭제

            Debug.Log("Q사용 체력 회복량: " + healAmount);
        }
        else
        {
            Debug.Log("분노 부족");
        }
    }

    // 체력 회복 함수
    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // 체력이 최대값을 초과하지 않도록 설정
        }
        Debug.Log("현재 체력: " + currentHealth);
    }

    // 분노 증가 함수 (기본 공격 시 호출)
    public void GainRage(bool isCritical)
    {
        int rageGain = isCritical ? ragePerCrit : ragePerAttack; // 치명타 여부에 따라 증가량 결정
        currentRage += rageGain;
        if (currentRage > maxRage)
        {
            currentRage = maxRage; // 분노가 최대값을 초과하지 않도록 설정
        }
        Debug.Log("분노 증가 현재 분노: " + currentRage);
    }

    // 공격 실행 함수 (치명타 확률 적용)
    public void PerformAttack()
    {
        bool isCritical = Random.value < critChance; // 치명타 확률 체크
        GainRage(isCritical); // 분노 증가 적용

        if (isCritical)
        {
            Debug.Log("치명타 발생!");
            anim.SetTrigger("CriticalHit"); // 치명타 애니메이션 실행
        }
        else
        {
            anim.SetTrigger("Attack"); // 일반 공격 애니메이션 실행
        }
    }

    // W 스킬 (비웃음) - 적 공격력 감소 및 방향 체크
    void UseWSkill()
    {
        float skillRange = 18.5f; // W 스킬 범위
        int attackReduction = 10; // 공격력 감소량

        Debug.Log("W 스킬 사용! 적 탐색 중...");

        //  OverlapSphere 디버깅 추가 (현재 위치 기준)
        Collider[] enemies = Physics.OverlapSphere(transform.position, skillRange, LayerMask.GetMask("Enemy"));

        if (enemies.Length == 0)
        {
            Debug.Log(" 범위 내 적 없음 → 감지 문제 확인 필요");
            return;
        }

        foreach (Collider enemyCollider in enemies)
        {
            Debug.Log(" 감지된 적: " + enemyCollider.name); // 감지된 적 이름 출력

            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy == null)
            {
                Debug.Log(enemyCollider.name + "에 EnemyController 없음");
                continue;
            }

            //  적의 공격력 감소
            enemy.ReduceAttackPower(attackReduction);
            Debug.Log(" " + enemy.name + "의 공격력이 감소");

            //  적이 등을 돌리고 있다면 추가 효과
            if (IsEnemyFacingAway(enemy.transform))
            {
                Debug.Log(" " + enemy.name + "이 등을 돌림 이동 속도 감소 가능");
            }
            else
            {
                Debug.Log(" " + enemy.name + "이 정면을 보고 있음");
            }
        }
    }

    // 적이 등을 돌리고 있는지 확인하는 함수 (Y축 무시)
    bool IsEnemyFacingAway(Transform enemy)
    {
        Vector3 directionToEnemy = (transform.position - enemy.position).normalized;
        directionToEnemy.y = 0; //  Y축 무시하고 수평 방향만 비교

        return Vector3.Dot(enemy.forward, directionToEnemy) < 0; // 음수면 등을 돌린 상태
    }


    //  E 스킬 (대회전 베기) - 돌진 시작
    void StartDash()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 마우스 클릭 위치 감지
        {
            Vector3 dashDirection = (hit.point - transform.position).normalized; // 이동 방향 계산
            eTargetPosition = transform.position + dashDirection * maxEDistance; // 최대 이동 거리 설정

            anim.SetTrigger("UseE"); // 애니메이션 실행
            isDashing = true; // 돌진 시작
        }
    }

    //  E 스킬 이동 처리
    void HandleDash()
    {
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, eTargetPosition, eSpeed * Time.deltaTime);

            // 목표 위치 도달 시 돌진 종료
            if (Vector3.Distance(transform.position, eTargetPosition) < 0.1f)
            {
                isDashing = false;
                Debug.Log("E 스킬 종료");
            }
        }
    }

    //  R 스킬 (불사의 분노) - 5초 동안 무적 & 분노 증가
    void UseRSkill()
    {
        if (isImmortal) return; // 이미 R 스킬이 활성화되어 있으면 실행 안 함

        isImmortal = true; //  5초간 무적 상태
        currentRage = Mathf.Min(currentRage + 50, maxRage); //  분노 50 추가 (최대 분노 초과 방지)

        Debug.Log("R 스킬 활성화! 5초 동안 무적 상태");
        StartCoroutine(EndRSkill()); //  5초 후 무적 해제
    }

    //  5초 후 R 스킬 해제
     IEnumerator EndRSkill()
    {
        yield return new WaitForSeconds(5f);
        isImmortal = false;
        Debug.Log("R 스킬 종료! 이제 정상적으로 피해를 받음");
    }

    //  체력 감소 시 무적 상태 체크
    public void TakeDamage(int damage)
    {
        if (isImmortal && currentHealth - damage <= 0)
        {
            currentHealth = 1; //  무적 상태일 때 최소 체력 유지
            Debug.Log("무적 상태! 체력 1로 유지됨");
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("트린다미어 사망!");
        Destroy(gameObject);
    }



}
