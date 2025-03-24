using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltState : IVayneState
{
    VayneState state;
    Vector3 lastPosition;
    public void EnterState(VayneState VS)
    {
        state = VS;
        state.character.AdjustMoveSpeed(90);
        state.character.AdjustATK(65);
        
        state.anim.SetBool("IsUlt", true);

        state.character.CurQCool = 1f;

        state.IsUlt = true;
        state.StartCoroutine(UltDelay());
    }
    public void UpdateState()
    {
        float speed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = state.transform.position;
        state.anim?.SetFloat("Walk", speed, 0.1f, Time.deltaTime);
        state.anim?.SetFloat("TumbleWalk", 0f);
    }
    public void ExitState()
    {
        state.character.AdjustMoveSpeed(-90);
        state.character.AdjustATK(-65);

        state.anim.SetBool("IsUlt", false);
        state.anim.SetTrigger("UltIdle");
    }
    IEnumerator UltDelay()
    {
        yield return new WaitForSeconds(12f);
        state.IsUlt = false;
        state.ChangeState(new DefaultState());
    }
}
