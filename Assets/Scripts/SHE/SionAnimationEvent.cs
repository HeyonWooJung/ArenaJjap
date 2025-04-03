using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SionAnimationEvent : MonoBehaviour
{
    [SerializeField] SionSkill sion;

    public void AttackResult()
    {
        sion.AttackResult();
    }
    public void RSkillExplosion()
    {
        sion.RSkillExplosion();
    }

}
