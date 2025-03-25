using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltState : IVayneState
{
    VayneState state;
    Vector3 lastPosition;
    float speed;
    int enteredNum;
    public void EnterState(VayneState VS,float time)
    {
        state = VS;
        state.character.AdjustMoveSpeed(90);
        state.character.AdjustATK(65);

        enteredNum++;
        if (enteredNum == 1)
        {
            time = 12;
        }
        state.anim.SetBool("IsUlt", true);

        state.character.CurQCool = 1f;

        state.IsUlt = true;

        state.StartCoroutine(UltTime(time));
    }
    public void UpdateState()
    {
        


        speed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
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
    IEnumerator UltTime(float remainTime)
    {
        yield return new WaitForSeconds(remainTime);
        state.IsUlt = false;
        enteredNum = 0;
        state.ChangeState(new DefaultState());
    }    
}
