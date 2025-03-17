using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 100; // 최대 체력
    public int currentHealth; // 현재 체력
    public int attackPower = 20; // 기본 공격력

    void Start()
    {
        currentHealth = maxHealth; // 시작 시 최대 체력
    }

    //  적이 피해를 받을 때 실행
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 적이 죽으면 실행
    private void Die()
    {
        Debug.Log(gameObject.name + " 사망!");
        Destroy(gameObject); // 적 삭제
    }

    //  공격력 감소 함수 (트린다미어 W 스킬에서 호출)
    public void ReduceAttackPower(int amount)
    {
        attackPower -= amount;
        if (attackPower < 0) attackPower = 0; // 공격력이 음수가 되지 않도록 설정
        Debug.Log(gameObject.name + "의 공격력 감소! 현재 공격력: " + attackPower);
    }
}
