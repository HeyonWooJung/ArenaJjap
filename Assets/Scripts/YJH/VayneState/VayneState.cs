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
    IVayneState currentState;

    [Header("애니메이션")]
    public Animator anim;

    private void Start()
    {
        //처음 상태
        //ChangeState();
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
}
