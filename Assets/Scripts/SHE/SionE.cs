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
    /// x는 데미지, y는 슬로우 값, z는 방깎
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
            //대충 E 때린다는 뜻
            //방깎두
            //닿으면 마지막에 끄고 StopAllCoroutines();
            //방깎하고 그만큼의 수치를 armorResetFloat에 넘기고, 그걸 받아서 돌려준다음 메인 스크립트에서 수정
        }
    }
}
