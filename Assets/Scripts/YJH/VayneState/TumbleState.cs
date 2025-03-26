using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TumbleState : IVayneState
{
    VayneState state;
    Coroutine revertRoutine;
        
    Vector3 targetLocation;
    Vector3 dir;
    Vector3 lastPosition;
    float tumbleSpeed;

    public TumbleState(Vector3 location)
    {
        targetLocation = location;
    }

    public void EnterState(VayneState VS, float temp)
    {
        state = VS;
        dir = (targetLocation - state.transform.position).normalized;
        state.Move(state.transform.position + dir * 4f);
        
        state.anim.SetTrigger("Tumble");
        state.anim.SetTrigger("TumbleIdle");
        VayneState.OnAutoAttackGlobal += OnAutoAttack;
        revertRoutine = state.StartCoroutine(TumbleTime());
        lastPosition = state.transform.position;
    }       

    public void UpdateState()
    {
        tumbleSpeed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = state.transform.position;
        state.anim?.SetFloat("TumbleWalk", tumbleSpeed, 0.1f, Time.deltaTime);
        state.anim?.SetFloat("Walk", 0f);
    }

    public void ExitState()
    {
        Debug.Log("구르기상태나감");
        state.anim.SetTrigger("Idle");
        if (revertRoutine != null)
        {
            state.StopCoroutine(revertRoutine);
        }

        VayneState.OnAutoAttackGlobal -= OnAutoAttack;
    }
    void OnAutoAttack()
    {
        state.ChangeState(new DefaultState());
    }
    IEnumerator TumbleTime()
    {
        yield return new WaitForSeconds(3f);
        state.ChangeState(new DefaultState());
    }
}
