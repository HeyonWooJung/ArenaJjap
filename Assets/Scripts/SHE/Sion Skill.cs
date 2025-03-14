using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SionSkill : MonoBehaviour
{
    Animator anim;
    
    [SerializeField] float skillQDamage;
    [SerializeField] float skillQAddDamage;
    [SerializeField] float qSkillChargeTime = 2f;
    [SerializeField] float qSkillAirbornTimeMin = 0.4f;
    [SerializeField] float qSkillAirbornTimeMax = 0.8f;
    [SerializeField] float qSkillStunTimeMix = 1f;
    [SerializeField] float qSkillStunTimeMax = 1.75f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
