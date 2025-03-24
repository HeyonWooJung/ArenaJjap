using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.GraphicsBuffer;

public interface IVayneState
{
    public void EnterState(VayneState VS) { }
    public void UpdateState() { }
    public void ExitState() { }
        
}

public class VayneState : PlayerController
{
    public static event System.Action OnAutoAttackGlobal;

    IVayneState currentState;

    [Header("애니메이션")]
    public Animator anim;

    private void Start()
    {
        ChangeState(new DefaultState());
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

    public override void AutoAttack(PlayerController target)
    {
        OnAutoAttackGlobal?.Invoke(); // 전역 이벤트 호출
        base.AutoAttack(target); // 실제 평타 처리
    }
}
