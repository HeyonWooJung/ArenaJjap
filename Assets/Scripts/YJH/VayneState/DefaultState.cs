using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : IVayneState
{
    VayneState state;
    float defaultSpeed;
    Vector3 lastPosition;
    public void EnterState(VayneState VS, float t)
    {
        state = VS;
        Debug.Log("DefaultState");
        state.anim.SetBool("IsUlt", false);
        state.anim.SetTrigger("Idle");
        lastPosition = state.transform.position;
    }

    public void UpdateState()
    {
        defaultSpeed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = state.transform.position;
        state.anim?.SetFloat("Walk", defaultSpeed, 0.1f, Time.deltaTime);
        state.anim?.SetFloat("TumbleWalk", 0f);
    }

    public void ExitState(){}
}
