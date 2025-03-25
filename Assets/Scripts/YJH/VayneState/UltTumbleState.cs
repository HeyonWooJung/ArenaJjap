using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltTumbleState : IVayneState
{
    VayneState state;
    Vector3 targetLocation;
    Vector3 lastPosition;
    float speed;

    public UltTumbleState(Vector3 location)
    {
        targetLocation = location;
    }

    public void EnterState(VayneState VS)
    {
        state = VS;

        
        state.anim.SetTrigger("Tumble");
        state.anim.SetTrigger("UltTumbleIdle");

        state.StartCoroutine(UltTumbleTime());

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
        state.anim.SetTrigger("UltIdle");
    }
    IEnumerator UltTumbleTime()
    {
        yield return new WaitForSeconds(3f);
        state.ChangeState(new UltState());
    }
}
