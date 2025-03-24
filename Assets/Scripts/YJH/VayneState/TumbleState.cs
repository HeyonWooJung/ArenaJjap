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
    float speed;

    public TumbleState(Vector3 location)
    {
        targetLocation = location;
    }

    public void EnterState(VayneState VS)
    {
        state = VS;

        dir = (targetLocation - state.transform.position).normalized;
        state.Move(state.transform.position + dir * 4f);
        
        state.anim.SetTrigger("Tumble");
        state.anim.SetTrigger("TumbleIdle");
        VayneState.OnAutoAttackGlobal += OnAutoAttack;
        revertRoutine = state.StartCoroutine(TumbleDelay());
    }       

    public void UpdateState()
    {
        speed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = state.transform.position;
        state.anim?.SetFloat("TumbleWalk", speed, 0.1f, Time.deltaTime);
        state.anim?.SetFloat("Walk", 0f);
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
    void OnAutoAttack()
    {
        state.ChangeState(new DefaultState());
    }
    IEnumerator TumbleDelay()
    {
        yield return new WaitForSeconds(3f);
        state.ChangeState(new DefaultState());
    }
}
