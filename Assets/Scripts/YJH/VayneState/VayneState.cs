using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVayneState
{
    public void EnterState(VayneState VS) { }
    public void UpdateState() { }
    public void ExitState() { }
        
}

public class VayneState : PlayerController
{
    public static event System.Action OnAutoAttackGlobal;

    Vector3 lastPosition;
    IVayneState currentState;

    public bool IsUlt = false;
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
    public override void SkillR(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {

        if (character.CurRCool <= 0)
        {            
            character.SetRCooldown();
            ChangeState(new UltState());
        }
        base.SkillR(isTargeting, isChanneling, target, location);
    }

    public override void SkillQ(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurQCool <= 0 && !IsUlt)
        {
            character.SetQCooldown();
            ChangeState(new TumbleState(location));
        }
        else if(character.CurQCool <= 0 && IsUlt)
        {
            character.SetQCooldown();
            ChangeState(new UltTumbleState(location));
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
