using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : IVayneState
{
    VayneState state;
    Vector3 lastPosition;
    float speed;

    public void EnterState(VayneState VS)
    {
        state = VS;
        lastPosition = state.transform.position;
    }

    public void UpdateState()
    {
        speed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = state.transform.position;
        state.anim?.SetFloat("Walk", speed, 0.1f, Time.deltaTime);
        state.anim?.SetFloat("TumbleWalk", 0f);
        
    }

    public void ExitState() { }
}
