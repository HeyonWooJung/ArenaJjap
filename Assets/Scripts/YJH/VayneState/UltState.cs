using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltState : IVayneState
{
    VayneState state;
    float ultSpeed;
    Vector3 lastPosition;
    public void EnterState(VayneState VS,float time)
    {
        state = VS;

        state.character.AdjustMoveSpeed(90);
        state.character.AdjustATK(65);
                        
        state.anim.SetBool("IsUlt", true);
        state.character.CurQCool = 1f;
        state.IsUlt = true;

        time = state.ultTime;
        state.StartCoroutine(UltTime(time));
    }
    public void UpdateState()
    {
        ultSpeed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = state.transform.position;
        state.anim?.SetFloat("Walk", ultSpeed, 0.1f, Time.deltaTime);
        state.anim?.SetFloat("TumbleWalk", 0f);        
    }
    public void ExitState()
    {
        state.character.AdjustMoveSpeed(-90);
        state.character.AdjustATK(-65);        
    }
    IEnumerator UltTime(float remainTime)
    {
        Debug.Log("남은시간" +remainTime);
        yield return new WaitForSeconds(remainTime);
        state.IsUlt = false;
        
        state.ChangeState(new DefaultState());
    }    
}
