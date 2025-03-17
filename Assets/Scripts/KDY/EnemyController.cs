using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 100; // �ִ� ü��
    public int currentHealth; // ���� ü��
    public int attackPower = 20; // �⺻ ���ݷ�

    public GameObject wSkillIconPrefab; // W ��ų ������ ������

    void Start()
    {
        currentHealth = maxHealth; // ���� �� �ִ� ü��
    }

    //  ���� ���ظ� ���� �� ����
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ���� ������ ����
    private void Die()
    {
        Debug.Log(gameObject.name + " ���!");
        Destroy(gameObject); // �� ����
    }

    //  ���ݷ� ���� �Լ� (Ʈ���ٹ̾� W ��ų���� ȣ��)
    public void ReduceAttackPower(int amount)
    {
        attackPower -= amount;
        if (attackPower < 0) attackPower = 0; // ���ݷ��� ������ ���� �ʵ��� ����
        Debug.Log(gameObject.name + "�� ���ݷ� ����! ���� ���ݷ�: " + attackPower);
    }
    /////////////////////////
    void ShowWSkillIcon()
    {
        if (wSkillIconPrefab == null)
        {
            Debug.LogWarning("W ��ų ������ �������� �������� ����");
            return;
        }

        //  ���� ������Ʈ�� �Ӹ� ���� ������ ����
        Vector3 iconPosition = transform.position + Vector3.up * 2.5f;
        GameObject icon = Instantiate(wSkillIconPrefab, iconPosition, Quaternion.identity);

        //  �������� �� ������Ʈ�� �ڽ����� ���� (����ٴϵ���)
        icon.transform.SetParent(transform);

        //  ���� �ð� �� �ڵ� ����
        Destroy(icon, 1.5f);
    }

}
