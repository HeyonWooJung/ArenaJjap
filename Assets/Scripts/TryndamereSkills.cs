using System.Collections;
using UnityEngine;

public class TryndamereSkills : MonoBehaviour
{

    public GameObject healEffectPrefab;
    private Animator anim;

    // ü�� �� �г� �ý���
    public int maxHealth = 100; // �ִ� ü��
    public int currentHealth; // ���� ü��
    public int maxRage = 5000; // �ִ� �г� 100
    public int currentRage; // ���� �г�
    public int ragePerAttack = 5; // �⺻ ���� �� �г� ������
    public int ragePerCrit = 10; // ġ��Ÿ �߻� �� �߰� �г� ������
    public float critChance = 0.25f; // ġ��Ÿ Ȯ�� (25%)


    //  E ��ų ���� ���� �߰�
    public float maxEDistance = 7f; // E ��ų �ִ� �̵� �Ÿ�
    public float eSpeed = 10f; // E ��ų �̵� �ӵ�
    private Vector3 eTargetPosition; // E ��ų ��ǥ ��ġ
    private bool isDashing = false; // ���� ������ ����

    // R ��ų ���� ����
    private bool isImmortal = false; // �Ҹ� ���� ����

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth; // ���� �� �ִ� ü��
        currentRage = 5000; // ���� �� �г� 0
    }

    void Update()
    {
        HandleSkills();
        HandleDash();
    }

    void HandleSkills()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q ��ų
        {
            anim.SetTrigger("UseQ"); // Q ��ų �ִϸ��̼� ����
            UseQSkill(); // Q ��ų ��� ����
        }
        if (Input.GetKeyDown(KeyCode.W)) // W ��ų 
        {
            anim.SetTrigger("UseW"); // W ��ų �ִϸ��̼� ����
            UseWSkill(); // W ��ų ��� ����
        }
        if (Input.GetKeyDown(KeyCode.E)) //  E ��ų
        {
            StartDash();
        }
        if (Input.GetKeyDown(KeyCode.R)) //  R ��ų
        {
            anim.SetTrigger("UseR");
            UseRSkill();
        }
    }

    // Q ��ų (���� ����) - ü�� ȸ��
    void UseQSkill()
    {
        if (currentRage > 0) // �г밡 ���� ���� ��� ����
        {
            int healAmount = currentRage / 2; // �г뷮�� ���ݸ�ŭ ü�� ȸ��
            Heal(healAmount); // ü�� ȸ�� ����
            currentRage = 0; // �г� �ʱ�ȭ
            anim.SetTrigger("UseQ");

            //  �� ����Ʈ ����
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(transform); // ĳ���͸� �θ�� �����Ͽ� �Բ� �̵��ϵ��� ��
            Destroy(healEffect, 2f); // 2�� �� �ڵ� ����

            Debug.Log("Q��� ü�� ȸ����: " + healAmount);
        }
        else
        {
            Debug.Log("�г� ����");
        }
    }

    // ü�� ȸ�� �Լ�
    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // ü���� �ִ밪�� �ʰ����� �ʵ��� ����
        }
        Debug.Log("���� ü��: " + currentHealth);
    }

    // �г� ���� �Լ� (�⺻ ���� �� ȣ��)
    public void GainRage(bool isCritical)
    {
        int rageGain = isCritical ? ragePerCrit : ragePerAttack; // ġ��Ÿ ���ο� ���� ������ ����
        currentRage += rageGain;
        if (currentRage > maxRage)
        {
            currentRage = maxRage; // �г밡 �ִ밪�� �ʰ����� �ʵ��� ����
        }
        Debug.Log("�г� ���� ���� �г�: " + currentRage);
    }

    // ���� ���� �Լ� (ġ��Ÿ Ȯ�� ����)
    public void PerformAttack()
    {
        bool isCritical = Random.value < critChance; // ġ��Ÿ Ȯ�� üũ
        GainRage(isCritical); // �г� ���� ����

        if (isCritical)
        {
            Debug.Log("ġ��Ÿ �߻�!");
            anim.SetTrigger("CriticalHit"); // ġ��Ÿ �ִϸ��̼� ����
        }
        else
        {
            anim.SetTrigger("Attack"); // �Ϲ� ���� �ִϸ��̼� ����
        }
    }

    // W ��ų (�����) - �� ���ݷ� ���� �� ���� üũ
    void UseWSkill()
    {
        float skillRange = 18.5f; // W ��ų ����
        int attackReduction = 10; // ���ݷ� ���ҷ�

        Debug.Log("W ��ų ���! �� Ž�� ��...");

        //  OverlapSphere ����� �߰� (���� ��ġ ����)
        Collider[] enemies = Physics.OverlapSphere(transform.position, skillRange, LayerMask.GetMask("Enemy"));

        if (enemies.Length == 0)
        {
            Debug.Log(" ���� �� �� ���� �� ���� ���� Ȯ�� �ʿ�");
            return;
        }

        foreach (Collider enemyCollider in enemies)
        {
            Debug.Log(" ������ ��: " + enemyCollider.name); // ������ �� �̸� ���

            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy == null)
            {
                Debug.Log(enemyCollider.name + "�� EnemyController ����");
                continue;
            }

            //  ���� ���ݷ� ����
            enemy.ReduceAttackPower(attackReduction);
            Debug.Log(" " + enemy.name + "�� ���ݷ��� ����");

            //  ���� ���� ������ �ִٸ� �߰� ȿ��
            if (IsEnemyFacingAway(enemy.transform))
            {
                Debug.Log(" " + enemy.name + "�� ���� ���� �̵� �ӵ� ���� ����");
            }
            else
            {
                Debug.Log(" " + enemy.name + "�� ������ ���� ����");
            }
        }
    }

    // ���� ���� ������ �ִ��� Ȯ���ϴ� �Լ� (Y�� ����)
    bool IsEnemyFacingAway(Transform enemy)
    {
        Vector3 directionToEnemy = (transform.position - enemy.position).normalized;
        directionToEnemy.y = 0; //  Y�� �����ϰ� ���� ���⸸ ��

        return Vector3.Dot(enemy.forward, directionToEnemy) < 0; // ������ ���� ���� ����
    }


    //  E ��ų (��ȸ�� ����) - ���� ����
    void StartDash()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // ���콺 Ŭ�� ��ġ ����
        {
            Vector3 dashDirection = (hit.point - transform.position).normalized; // �̵� ���� ���
            eTargetPosition = transform.position + dashDirection * maxEDistance; // �ִ� �̵� �Ÿ� ����

            anim.SetTrigger("UseE"); // �ִϸ��̼� ����
            isDashing = true; // ���� ����
        }
    }

    //  E ��ų �̵� ó��
    void HandleDash()
    {
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, eTargetPosition, eSpeed * Time.deltaTime);

            // ��ǥ ��ġ ���� �� ���� ����
            if (Vector3.Distance(transform.position, eTargetPosition) < 0.1f)
            {
                isDashing = false;
                Debug.Log("E ��ų ����");
            }
        }
    }

    //  R ��ų (�һ��� �г�) - 5�� ���� ���� & �г� ����
    void UseRSkill()
    {
        if (isImmortal) return; // �̹� R ��ų�� Ȱ��ȭ�Ǿ� ������ ���� �� ��

        isImmortal = true; //  5�ʰ� ���� ����
        currentRage = Mathf.Min(currentRage + 50, maxRage); //  �г� 50 �߰� (�ִ� �г� �ʰ� ����)

        Debug.Log("R ��ų Ȱ��ȭ! 5�� ���� ���� ����");
        StartCoroutine(EndRSkill()); //  5�� �� ���� ����
    }

    //  5�� �� R ��ų ����
     IEnumerator EndRSkill()
    {
        yield return new WaitForSeconds(5f);
        isImmortal = false;
        Debug.Log("R ��ų ����! ���� ���������� ���ظ� ����");
    }

    //  ü�� ���� �� ���� ���� üũ
    public void TakeDamage(int damage)
    {
        if (isImmortal && currentHealth - damage <= 0)
        {
            currentHealth = 1; //  ���� ������ �� �ּ� ü�� ����
            Debug.Log("���� ����! ü�� 1�� ������");
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
        Debug.Log("Ʈ���ٹ̾� ���!");
        Destroy(gameObject);
    }



}
