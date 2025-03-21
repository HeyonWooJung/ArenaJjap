using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    public Character character;
    public string enemyTag;
    NavMeshAgent agent;

    CommandBase toExecute;

    Ray ray;
    RaycastHit hit;
    bool canAA = true;

    // Start is called before the first frame update
    void Start()
    {
        Character temp = character;
        character = ScriptableObject.CreateInstance<Character>();
        character.InitCharacter(temp);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = character.MoveSpeed * 0.01f;
        agent.angularSpeed = 100000;
        agent.acceleration = 100000;
        character.OnMoveSpeedChanged += ApplyMoveSpeed;
        //Cursor.SetCursor(cursorTexture, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        StartCoroutine(HpRegen());
    }

    private void OnDestroy()
    {
        character.OnMoveSpeedChanged -= ApplyMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if ((character.CurState == State.Stun && character.CurState == State.Airborne) == false)
        {
            if (Input.GetMouseButton(1))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.CompareTag(enemyTag) && Vector3.Distance(hit.point, transform.position) <= character.Range * 0.01f)
                    {
                        if (canAA)
                        {
                            toExecute = new AutoAttackCommand(this, hit.transform.GetComponent<PlayerController>());
                            toExecute.Execute();
                        }
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
                    toExecute = new SkillQCommand(this, false, false, hit.transform?.GetComponent<PlayerController>(), hit.point);
                    toExecute.Execute();
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    toExecute = new SkillWCommand(this, true, false, hit.transform?.GetComponent<PlayerController>(), hit.point);
                    toExecute.Execute();
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    toExecute = new SkillECommand(this, true, false, hit.transform?.GetComponent<PlayerController>(), hit.point);
                    toExecute.Execute();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    toExecute = new SkillRCommand(this, false, false, hit.transform?.GetComponent<PlayerController>(), hit.point);
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
        else if (character.CurState == State.Root)
        {
            agent.ResetPath();
        }
    }

    IEnumerator HpRegen()
    {
        while (character.CurHP > 0)
        {
            yield return new WaitForSeconds(1f);
            character.Heal(character.HpRegen);
        }
    }

    public IEnumerator AAHandle()
    {
        canAA = false;
        yield return new WaitForSeconds(1 / character.AttackSpeed);
        canAA = true;
    }

    private void FixedUpdate()
    {
        if (character.CurQCool >= 0)
        {
            character.CurQCool -= Time.deltaTime;
            if (character.CurQCool <= 0)
            {
                character.CurQCool = 0;
            }
        }
        if (character.CurWCool >= 0)
        {
            character.CurWCool -= Time.deltaTime;
            if (character.CurWCool <= 0)
            {
                character.CurWCool = 0;
            }
        }
        if (character.CurECool >= 0)
        {
            character.CurECool -= Time.deltaTime;
            if (character.CurECool <= 0)
            {
                character.CurECool = 0;
            }
        }
        if (character.CurRCool >= 0)
        {
            character.CurRCool -= Time.deltaTime;
            if (character.CurRCool <= 0)
            {
                character.CurRCool = 0;
            }
        }
    }

    public void Stop()
    {
        agent.ResetPath();
        toExecute = null;
    }

    public void ApplyMoveSpeed()
    {
        agent.speed = character.MoveSpeed * 0.01f;
    }

    public virtual void Move(Vector3 pos)
    {
        //움직이다
        if (character.CurState != State.Root)
        {
            agent.SetDestination(pos);
        }
    }

    public virtual void AutoAttack(PlayerController target)
    {
        StartCoroutine(AAHandle());
        Debug.Log("AA " + target.name);
        //평타
        float damage = character.ATK;

        if (character.CritChance >= Random.Range(0f, 1f)) //치명타
        {
            damage *= character.CritDamage;
        }

        target.character.TakeDamage(damage, false, character.Lethality, character.ArmorPenetration);
    }

    public virtual void SkillQ(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurQCool <= 0)
        {
            //대충 스킬쓰기
            character.SetQCooldown();
            Debug.Log("Q사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("Q사용불가. 쿨타임: " + character.CurQCool);
        }
    }

    public virtual void SkillW(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurWCool <= 0)
        {
            //대충 스킬쓰기
            character.SetWCooldown();
            Debug.Log("W사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("W사용불가. 쿨타임: " + character.CurWCool);
        }
    }

    public virtual void SkillE(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurECool <= 0)
        {
            //대충 스킬쓰기
            character.SetECooldown();
            Debug.Log("E사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("E사용불가. 쿨타임: " + character.CurECool);
        }
    }

    public virtual void SkillR(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (character.CurRCool <= 0)
        {
            //대충 스킬쓰기
            character.SetRCooldown();
            Debug.Log("R사용");
        }
        else
        {
            //못쓴다하기
            Debug.Log("R사용불가. 쿨타임: " + character.CurRCool);
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
            pos.y = transform.position.y;
            pos = transform.position + Vector3.ClampMagnitude(pos - transform.position, 4);
            agent.ResetPath();
            if (NavMesh.SamplePosition(pos, out NavMeshHit navhit, 3, NavMesh.AllAreas))
            {
                Debug.Log($"pos: {pos} | nHit: {navhit.position}");
                agent.SetDestination(navhit.position);
                transform.position = navhit.position;
            }
            //transform.position += Vector3.ClampMagnitude(pos - transform.position, 4);
        }
        else
        {
            //못쓴다고 하기
        }
    }

    public void ResetCharacter()
    {
        character.ResetState();
        StartCoroutine(HpRegen());
    }

    public void ApplySlow(float percent, float time)
    {
        StartCoroutine(SlowDown(percent, time));
    }

    IEnumerator SlowDown(float percent, float time)
    {
        int tempSpeed = (int)(character.MoveSpeed * percent);
        character.AdjustMoveSpeed(-tempSpeed);
        yield return new WaitForSeconds(time);
        character.AdjustMoveSpeed(tempSpeed);
    }
}
