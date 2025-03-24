using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleState : IVayneState
{
    VayneState state;
    Vector3 targetLocation;
    Coroutine revertRoutine;

    public TumbleState(Vector3 location)
    {
        targetLocation = location;
    }

    public void EnterState(VayneState VS)
    {
        state = VS;

        Vector3 dir = (targetLocation - state.transform.position).normalized;        
        state.Move(state.transform.position + dir * 3);
        state.anim.SetTrigger("Tumble");


        VayneState.OnAutoAttackGlobal += OnAutoAttack;

        revertRoutine = state.StartCoroutine(RevertAfterDelay());
    }

    public void ExitState()
    {
        state.anim.SetTrigger("Idle");
        if (revertRoutine != null)
        {
            state.StopCoroutine(revertRoutine);
        }


        VayneState.OnAutoAttackGlobal -= OnAutoAttack;
    }

    public void UpdateState() { }

    void OnAutoAttack()
    {
        Debug.Log("ÆòÅ¸ °¨ÁöµÊ ¡æ DefaultState º¹±Í");
        state.ChangeState(new DefaultState());
    }

    IEnumerator RevertAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        state.ChangeState(new DefaultState());
    }
}
