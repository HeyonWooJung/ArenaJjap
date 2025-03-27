using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyzeAAScript : MonoBehaviour
{
    Transform target;
    string enemyTag;
    float damage;
    int lethal;
    float aPen;
    Character character;

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            transform.LookAt(target);
            transform.Translate(Vector3.forward * 0.7f);
        }
    }

    public void GetDmg(Transform tar ,float dmg, int let, float ap, string tag, Character chara)
    {
        target = tar;
        damage = dmg;
        lethal = let;
        aPen = ap;
        enemyTag = tag;
        character = chara;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController target) && target.CompareTag(enemyTag))
        {
            target.character.TakeDamage(character, damage, false, lethal, aPen);
            Debug.Log(target.name + " 맞음 | " + damage + " 의 데미지");
            Destroy(gameObject);
        }
    }
}
