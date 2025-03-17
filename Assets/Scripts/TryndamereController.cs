using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryndamereController : MonoBehaviour
{
    private Animator anim; // �ִϸ����� ������Ʈ
    private bool isMoving; // �̵� ����
    private float attackCooldown = 0.5f; // ���� �� ������
    private float lastAttackTime; // ������ ���� �ð�
    private Vector3 targetPosition; // �̵� ��ǥ ��ġ
    public float moveSpeed = 5f; // �̵� �ӵ�

    void Start()
    {
        anim = GetComponent<Animator>(); // Animator ��������
    }

    void Update()
    {
        TrynMovement(); // �̵� ó��
        TrynAttack(); // ���� ó��
        MoveCharacter(); // ���� �̵� ����
        TrynSkills(); // QWER ��ų ���� 
    }

    void TrynMovement()
    {
        if (Input.GetMouseButtonDown(1)) // ���콺 ��Ŭ�� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast�� ����Ͽ� Ŭ���� ��ġ ã��
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point; // ��ǥ ��ġ ����
                isMoving = true;
                anim.SetFloat("isMovingBlend", 1f); // ���� Ʈ�� �� ���� (�̵� �ִϸ��̼� ����)
            }
        }
    }

    void MoveCharacter()
    {
        if (isMoving)
        {
            // ��ǥ ��ġ�� ���ϴ� ���� ���
            Vector3 direction = (targetPosition - transform.position).normalized;

            // ĳ���Ͱ� �ش� ������ �ٶ󺸵��� ȸ��
            if (direction != Vector3.zero)
            {
                anim.SetTrigger("Idle");
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // ���� ��ġ���� ��ǥ ��ġ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // �ִϸ��̼� ����Ʈ�� �� �ε巴�� ����
            float blendValue = Mathf.Lerp(anim.GetFloat("isMovingBlend"), 1f, Time.deltaTime * 5f);
            anim.SetFloat("isMovingBlend", blendValue);

            // ��ǥ ��ġ�� �����ϸ� �̵� ����
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
        else
        {
            // �̵��� ������ �ִϸ��̼� ���� �ε巴�� ���� (Idle�� ��ȯ)
            float blendValue = Mathf.Lerp(anim.GetFloat("isMovingBlend"), 0f, Time.deltaTime * 5f);
            anim.SetFloat("isMovingBlend", blendValue);
        }
    }

    void TrynAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown) // ���� ��Ÿ�� üũ
        {
            anim.SetTrigger("Attack"); // ���� �ִϸ��̼� ����
            lastAttackTime = Time.time; // ������ ���� �ð� ������Ʈ
        }
    }

    void TrynSkills()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q ��ų �Է�
        {
            anim.SetTrigger("UseQ");
        }
        if (Input.GetKeyDown(KeyCode.W)) // W ��ų �Է�
        {
            anim.SetTrigger("UseW");
        }
        if (Input.GetKeyDown(KeyCode.E)) // E ��ų �Է�
        {
            anim.SetTrigger("UseE");
        }
        if (Input.GetKeyDown(KeyCode.R)) // R ��ų �Է�
        {
            anim.SetTrigger("UseR");
        }
    }
}
