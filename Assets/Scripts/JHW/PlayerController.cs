using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Photon.Pun;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[Serializable]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour, IPunObservable
{
    public Character character;
    public string enemyTag;
    NavMeshAgent agent;
    public PhotonView pv;

    Queue<CommandBase> toExecute;
    CommandBase curCommand;

    Ray ray;
    RaycastHit hit;
    bool canAA = true;

    bool canCancel = true;

    // Start is called before the first frame update
    public virtual void Start()
    {
        toExecute = new Queue<CommandBase>();
        Character temp = character;
        character = ScriptableObject.CreateInstance<Character>();
        character.InitCharacter(temp);
        pv = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = character.MoveSpeed * 0.01f;
        agent.angularSpeed = 100000;
        agent.acceleration = 100000;
        character.OnMoveSpeedChanged += ApplyMoveSpeed;
        //Cursor.SetCursor(cursorTexture, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        StartCoroutine(HpRegen());
        StartCoroutine(Execution());
    }

    private void OnDestroy()
    {
        character.OnMoveSpeedChanged -= ApplyMoveSpeed;
    }

    public static byte[] SerializeCommandInfo(CommandBase command)
    {
        BinaryFormatter binaryF = new BinaryFormatter();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryF.Serialize(memoryStream, command);

            return memoryStream.ToArray();
        }
    }

    public static CommandBase DeserializeCommandInfo(byte[] dataStream)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryF = new BinaryFormatter();

            memoryStream.Write(dataStream, 0, dataStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (CommandBase)binaryF.Deserialize(memoryStream);
        }
    }

    IEnumerator Execution()
    {
        while(true)
        {
            yield return new WaitUntil(() => canCancel);
            yield return new WaitUntil(() => toExecute.Count > 0);
            Debug.Log("tete: " + toExecute.Count);
            if (curCommand != null)
            {
                curCommand.Cancel();
            }
            if (toExecute.Count > 0)
            {
                curCommand = toExecute?.Dequeue();
            }
            if (curCommand != null)
            {
                curCommand.pv.RPC("Execute", RpcTarget.All);
                //pv.RPC("Executer", RpcTarget.All, SerializeCommandInfo(curCommand)); 
                if (curCommand.Delay > 0)
                {
                    StartCoroutine(Delay(curCommand.Delay));
                }
            }
        }
    }

    [PunRPC]
    void Executer(byte[] stream)
    {
        CommandBase command = DeserializeCommandInfo(stream);
        command.Execute();
        Debug.Log("발사발사");
    }

    IEnumerator Delay(float time)
    {
        canCancel = false;
        yield return new WaitForSeconds(time);
        canCancel = true;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (pv.IsMine)
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
                                toExecute.Enqueue(new AutoAttackCommand(this, 0, hit.transform.GetComponent<PlayerController>()));
                            }
                        }
                        else
                        {
                            //toExecute.Enqueue(new MoveCommand(this, 0, hit.point));
                            Move(hit.point);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        toExecute.Enqueue(new SkillQCommand(this, 0.2f, false, false, hit.transform?.GetComponent<PlayerController>(), hit.point));
                    }
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        toExecute.Enqueue(new SkillWCommand(this, 0.1f, true, false, hit.transform?.GetComponent<PlayerController>(), hit.point));
                    }
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        toExecute.Enqueue(new SkillECommand(this, 0.1f, true, false, hit.transform?.GetComponent<PlayerController>(), hit.point));
                    }
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        toExecute.Enqueue(new SkillRCommand(this, 0, false, false, hit.transform?.GetComponent<PlayerController>(), hit.point));
                    }
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        toExecute.Enqueue(new RushCommand(this, 0));
                    }
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        toExecute.Enqueue(new FlashCommmand(this, 0, hit.point));
                    }
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Stop();
                    toExecute.Clear();
                }
            }
            else if (character.CurState == State.Root)
            {
                agent.ResetPath();
            }
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

        if (character.CritChance >= UnityEngine.Random.Range(0f, 1f)) //치명타
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
