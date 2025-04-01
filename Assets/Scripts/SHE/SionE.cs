using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SionE : MonoBehaviour
{
    string enemyTag;
    string enemyTag2;
    [SerializeField] float moveSpeed;
    float damage;
    float slowPercentage;
    float armorDown = 0.4f;
    float armorResetFloat;
    readonly WaitForSeconds eSkillDisalbe = new WaitForSeconds(2f);

    /// <summary>
    /// x�� ������, y�� ���ο� ��, z�� ���
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void InitializeSkill(float x, float y, float z)
    {
        damage = x;
        slowPercentage = y;
        armorDown = z;
        StartCoroutine(UsingESkill());
    }

    IEnumerator UsingESkill()
    {
        transform.position = transform.parent.position;
        transform.Translate((transform.forward * moveSpeed) * Time.deltaTime);
        yield return eSkillDisalbe;
        gameObject.SetActive(false);
    }

    public void SetEnemy(string[] tag)
    {
        enemyTag = tag[0];
        enemyTag2 = tag[1];
    }
    public float ResetEnemyArmor()
    {
        return armorResetFloat;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(enemyTag != null && enemyTag2 != null)
        {
            //���� E �����ٴ� ��
            //����
            //������ �������� ���� StopAllCoroutines();
            //����ϰ� �׸�ŭ�� ��ġ�� armorResetFloat�� �ѱ��, �װ� �޾Ƽ� �����ش��� ���� ��ũ��Ʈ���� ����
        }
    }
}
