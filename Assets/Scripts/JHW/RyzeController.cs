using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyzeController : PlayerController
{
    [SerializeField] GameObject QObj;
    [SerializeField] GameObject WObj;
    [SerializeField] GameObject EObj;
    [SerializeField] GameObject RObj;
    public override void SkillQ(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurQCool <= 0)
        {
            //대충 스킬쓰기
            character.SetQCooldown();
            Debug.Log("Q사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("Q사용불가. 쿨타임: " + character.CurQCool);
        }
    }
    public override void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurWCool <= 0)
        {
            //대충 스킬쓰기
            character.SetWCooldown();
            Debug.Log("W사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("W사용불가. 쿨타임: " + character.CurWCool);
        }
    }
    public override void SkillE(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurECool <= 0)
        {
            //대충 스킬쓰기
            character.SetECooldown();
            Debug.Log("E사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("E사용불가. 쿨타임: " + character.CurECool);
        }
    }
    public override void SkillR(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurRCool <= 0)
        {
            //대충 스킬쓰기
            character.SetRCooldown();
            Debug.Log("R사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("R사용불가. 쿨타임: " + character.CurRCool);
        }
    }
}
