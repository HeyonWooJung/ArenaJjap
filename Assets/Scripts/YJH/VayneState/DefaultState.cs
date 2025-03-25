using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : IVayneState
{
    VayneState state;
    float defaultSpeed;
    Vector3 lastPosition;
    public void EnterState(VayneState VS)
    {
        state = VS;
        Debug.Log("DefaultState");
    }

    public void UpdateState()
    {        
        //lastPosition = state.transform.position;
        //defaultSpeed = (state.transform.position - lastPosition).magnitude / Time.deltaTime;
        //state.anim?.SetFloat("Walk", defaultSpeed, 0.1f, Time.deltaTime);
        //state.anim?.SetFloat("TumbleWalk", 0f);     
    }

    public void ExitState() 
    {
        Debug.Log("DefaultState Exit");
    }
}
