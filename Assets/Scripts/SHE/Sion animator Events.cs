using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SionanimatorEvents : MonoBehaviour
{
    SionSkill skill;
    
    void Start()
    {
        skill = GetComponentInParent<SionSkill>();
    }

    public void RSkillExplosion()
    {
        skill.RSkillExplosion();
    }
    public void AttackResult()
    {
        skill.AttackResult();
    }

   

}
