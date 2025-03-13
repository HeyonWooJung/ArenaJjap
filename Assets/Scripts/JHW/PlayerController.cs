using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Character character;
    NavMeshAgent agent;

    CommandBase toExecute;

    MoveCommand moveCommand;
    AutoAttackCommand autoAttackCommand;
    SkillQCommand qCommand;
    SkillWCommand wCommand;
    SkillECommand eCommand;
    SkillRCommand rCommand;
    RushCommand rushCommand;
    FlashCommmand flashCommmand;

    Ray ray;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        character.InitCharacter();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = character.MoveSpeed * 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    toExecute = new AutoAttackCommand(this, hit.transform.GetComponent<PlayerController>().character);
                    toExecute.Execute();
                }
                else
                {
                    toExecute = new MoveCommand(this, hit.point);
                    toExecute.Execute();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                toExecute = new SkillQCommand(this, false, false, hit.transform.GetComponent<PlayerController>()?.character, hit.point);
                toExecute.Execute();
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                toExecute = new SkillWCommand(this, false, false, hit.transform.GetComponent<PlayerController>()?.character, hit.point);
                toExecute.Execute();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                toExecute = new SkillECommand(this, false, false, hit.transform.GetComponent<PlayerController>()?.character, hit.point);
                toExecute.Execute();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                toExecute = new SkillRCommand(this, false, false, hit.transform.GetComponent<PlayerController>()?.character, hit.point);
                toExecute.Execute();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                toExecute = new RushCommand(this);
                toExecute.Execute();
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                toExecute = new FlashCommmand(this, hit.point);
                toExecute.Execute();
            }
        }
    }
    public virtual void Move(Vector3 pos)
    {
        //움직이다 sibal
        agent.SetDestination(pos);
    }

    public virtual void AutoAttack(Character target)
    {
        Debug.Log("AA " + target.name);
        //평타
        float damage = character.ATK;

        if (character.CritChance >= Random.Range(0f, 1f)) //치명타
        {
            damage *= character.CritDamage;
        }

        target.TakeDamage(damage, false, character.Lethality, character.ArmorPenetration);
    }

    public virtual void SkillQ(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurQCool >= 0)
        {
            //대충 스킬쓰기
            character.SetQCooldown();
        }
        else
        {
            //못쓴다하기
        }
    }

    public virtual void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurWCool >= 0)
        {
            //대충 스킬쓰기
            character.SetWCooldown();
        }
        else
        {
            //못쓴다하기
        }
    }

    public virtual void SkillE(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurECool >= 0)
        {
            //대충 스킬쓰기
            character.SetECooldown();
        }
        else
        {
            //못쓴다하기
        }
    }

    public virtual void SkillR(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (character.CurRCool >= 0)
        {
            //대충 스킬쓰기
            character.SetRCooldown();
        }
        else
        {
            //못쓴다하기
        }
    }

    public void Rush()
    {
        //도주
        if (character.CanRush)
        {
            StartCoroutine(DoRush());
        }
        else
        {
            //못쓴다고 하기
        }
    }

    IEnumerator DoRush()
    {
        character.SetCanRush(false);
        int tempSpeed = (int)(character.MoveSpeed * 0.4f);
        character.AdjustMoveSpeed(tempSpeed);
        yield return new WaitForSeconds(2f);
        character.AdjustMoveSpeed(-tempSpeed);
    }

    public void Flash(Vector3 pos)
    {
        //점멸
        if (character.CanFlash)
        {
            agent.ResetPath();
            transform.position = pos;
        }
        else
        {
            //못쓴다고 하기
        }
    }

}
