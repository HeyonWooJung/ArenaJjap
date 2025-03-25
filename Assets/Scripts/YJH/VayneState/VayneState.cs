using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVayneState
{
    public void EnterState(VayneState VS, float time = 0);
    public void UpdateState();
    public void ExitState();        
}

public class VayneState : PlayerController
{
    public static event System.Action OnAutoAttackGlobal;

    IVayneState currentState;

    public bool IsUlt = false;

    public float ultTime = 16;
    [Header("애니메이션")]
    public Animator anim;

    public override void Start()
    {
        ChangeState(new DefaultState());
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        currentState.UpdateState();

        if (IsUlt)
        {
            ultTime -= Time.deltaTime;
            Debug.Log(ultTime);
        }        
    }

    public void ChangeState(IVayneState newState)
    {
        if(currentState != null)
        {
            currentState.ExitState();
        }
        currentState = newState;
        currentState.EnterState(this);
    }
    public override void SkillQ(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurQCool <= 0 && !IsUlt)
        {
            Debug.Log("q눌림");
            character.SetQCooldown();
            ChangeState(new TumbleState(location));
        }
        else if (character.CurQCool <= 0 && IsUlt)
        {
            character.SetQCooldown();
            ChangeState(new UltTumbleState(location));
        }
    }

    public override void SkillE(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurECool <= 0)
        {
            Debug.Log("E스킬사용가능");
            if (target != null && target.CompareTag(enemyTag))
            {
                Debug.Log("E스킬 사용");
                anim.SetTrigger("ESkill");
                Vector3 look = target.transform.position;
                look.y = transform.position.y;
                transform.LookAt(look, transform.forward);

                target.character.TakeDamage(190 + (character.ATK * 0.5f), false, character.Lethality, character.ArmorPenetration);


                target.transform.position = Vector3.back;
                character.SetECooldown();
            }
        }
    }
    public override void SkillR(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurRCool <= 0)
        {            
            character.SetRCooldown();
            ChangeState(new UltState());
        }
    }

    
    

    public override void AutoAttack(PlayerController target)
    {
        if(currentState is TumbleState)
        {
            character.AdjustATK(115.14f);
        }

        OnAutoAttackGlobal?.Invoke();
        base.AutoAttack(target); 
    }
}
