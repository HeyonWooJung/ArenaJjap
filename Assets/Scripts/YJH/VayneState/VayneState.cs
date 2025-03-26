using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVayneState
{
    public void EnterState(VayneState VS, float time = 0);
    public void UpdateState();
    public void ExitState();        
}

public class VayneState : PlayerController
{
    public static event System.Action OnAutoAttackGlobal;

    IVayneState currentState;

    public bool IsUlt = false;

    public float ultTime = 16;
    [Header("애니메이션")]
    public Animator anim;

    //마지막으로 때린놈
    GameObject lastAttackedTarget = null;
    int hitCount = 0;
    public override void Start()
    {
        ChangeState(new DefaultState());
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        currentState.UpdateState();

        if (IsUlt)
        {
            ultTime -= Time.deltaTime;
            Debug.Log(ultTime);
        }        
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
    public override void SkillQ(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurQCool <= 0 && !IsUlt)
        {
            Debug.Log("q눌림");
            character.SetQCooldown();
            ChangeState(new TumbleState(location));
        }
        else if (character.CurQCool <= 0 && IsUlt)
        {
            character.SetQCooldown();
            ChangeState(new UltTumbleState(location));
        }
    }

    public override void SkillE(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurECool <= 0)
        {
            Debug.Log("E스킬사용가능");
            if (target != null && target.CompareTag(enemyTag))
            {
                anim.SetTrigger("ESkill");

                Vector3 look = target.transform.position;
                look.y = transform.position.y;
                transform.LookAt(look, transform.forward);

                // 기본 피해
                target.character.TakeDamage(190 + (character.ATK * 0.5f), false, character.Lethality, character.ArmorPenetration);

                // 방향 계산
                Vector3 dir = target.transform.position - transform.position;
                dir.y = 0f; // Y 제거
                dir = dir.normalized;
                float knockbackDistance = 4.7f;
                float knockbackSpeed = 20f;

                Rigidbody targetRb = target.GetComponent<Rigidbody>();
                if (targetRb != null)
                {
                    target.StartCoroutine(KnockbackWithWallCheck(target, dir, knockbackDistance, knockbackSpeed));
                }

                //character.SetECooldown();

                
                if(currentState is UltState)
                {
                    anim.SetTrigger("UltIdle");
                }
                else if(currentState is UltTumbleState)
                {
                    anim.SetTrigger("UltTumbleIdle");
                }
                else if(currentState is TumbleState)
                {
                    anim.SetTrigger("TumbleIdle");
                }
                else
                {
                    anim.SetTrigger("Idle");
                }
            }
        }
    }

    public override void SkillR(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurRCool <= 0)
        {            
            character.SetRCooldown();
            ChangeState(new UltState());
        }
    }      

    public override void AutoAttack(PlayerController target)
    {
        Vector3 look = target.transform.position;
        look.y = transform.position.y;
        transform.LookAt(look, transform.forward);
        anim.SetTrigger("Attack");
        if(currentState is TumbleState || currentState is UltTumbleState)
        {
            character.AdjustATK(115.14f);
        }
        OnAutoAttackGlobal?.Invoke();
        base.AutoAttack(target);

        VayneWSkill(target);
    }
    public void VayneWSkill(PlayerController target)
    {
        GameObject currentTarget = target.gameObject;

        if (lastAttackedTarget == currentTarget)
        {
            hitCount++;
            Debug.Log(hitCount + " 스택");
        }
        else
        {
            hitCount = 1;
            lastAttackedTarget = currentTarget;
        }

        if (hitCount == 3)
        {

            Debug.Log("추뎀적용");

            hitCount = 0;
        }
    }
    private IEnumerator KnockbackWithWallCheck(PlayerController target, Vector3 dir, float distance, float speed)
    {
        float pushed = 0f;
        float delta;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;

        while (pushed < distance)
        {
            delta = speed * Time.fixedDeltaTime;
            Vector3 move = dir * delta;

            // 벽 충돌 체크
            if (Physics.Raycast(target.transform.position, dir, out RaycastHit hit, 0.5f))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    Debug.Log("벽 충돌 감지");
                    target.character.SetState(State.Stun);
                    yield return new WaitForSeconds(1.5f);
                    target.character.SetState(State.Neutral);

                    target.character.TakeDamage(285 + (character.ATK * 0.75f), false, character.Lethality, character.ArmorPenetration);
                    yield break;
                }
            }

            rb.MovePosition(rb.position + move);
            pushed += delta;
            yield return new WaitForFixedUpdate();
        }
    }
}
