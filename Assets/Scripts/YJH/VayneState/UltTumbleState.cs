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

        state.StartCoroutine(UltTumbleDelay());

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
        Debug.Log("fuck");
        state.anim.SetTrigger("UltIdle");
    }
    IEnumerator UltTumbleDelay()
    {
        yield return new WaitForSeconds(3f);
        state.ChangeState(new UltState());
    }
}
