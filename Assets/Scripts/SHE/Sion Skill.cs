using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SionSkill : PlayerController
{
    [SerializeField] Animator anim;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] bool debuggingMode;
    readonly WaitForSeconds skillCheckTime = new WaitForSeconds(0.05f);
    [SerializeField] float attackSpeedAnimFloat;
    [SerializeField] float moveSpeedAnimFloat;
    float attackSppedAnimGetFloat;
    float moveSpeedAnimGetFloat;
    Rigidbody enemyRb;
    [SerializeField] float powerOfAirborne;
    //[SerializeField] PlayerController playerController;
    [SerializeField] GameObject skillCanvas;
    [Header("패시브")]
    [SerializeField] int passiveCount;
    [SerializeField] float passiveAtkSpeed = 1.75f;
    [SerializeField] readonly WaitForSeconds passiveDecreaseHPTime = new WaitForSeconds(0.264f);
    [SerializeField] readonly WaitForSeconds passiveDecreaseMovementSpeed = new WaitForSeconds(1.5f);
    [SerializeField] float passiveDecreaseHPFloat = 1.3f;
    [SerializeField] float passiveIncreasingMovementSpeedPercentage = 1.5f;
    [SerializeField] float passiveAddDamage = 0.1f;
    [SerializeField] int passiveAddLifeStill = 100;
    [SerializeField] float passiveTime;
    [SerializeField] bool isPassiveNow = false;
    [SerializeField] bool usedPassiveSkill = false;
    [SerializeField] float originalAtkSpeed;
    [SerializeField] float originalHPRegen;
    [SerializeField] bool canActivePassive = true;

    [Header("Q스킬")]
    [SerializeField] float qSkillMinFixedDamage = 120;
    [SerializeField] float qSkillMaxFixedDamage = 350;
    [SerializeField] float qSkillMinAddDamage = 0.8f;
    [SerializeField] float qSkillMaxAddDamage = 2.4f;
    [SerializeField] float qSkillChargeTime = 2f;
    [SerializeField] float qSkillAirborneTimeMin = 0.3f;
    [SerializeField] float qSkillAirborneTimeMax = 0.6f;
    [SerializeField] float qSkillStunTimeMix = 1f;
    [SerializeField] float qSkillStunTimeMax = 1.75f;
    [SerializeField] float qSkillCurDamage;
    [SerializeField] float qSkillCurAirborne;
    [SerializeField] float qSkillCurStun;
    [SerializeField] int qSkillSlowPercentage = 75;
    float qSkillTimer;
    float maxChargeTime = 2f;
    bool qSkillCharging = false;
    [SerializeField] float qSkillMinLength = 2;
    [SerializeField] float qSkillMaxLength = 8;
    [SerializeField] float qSkillWidth = 4;
    float chargeRatio;
    float skillLength;  
    [SerializeField] Image qSKillImage;
    [SerializeField] Color qSkillOriginalPanelA;
    [SerializeField] float qSkillMaxAlphaFloat = 0.98f;
    [SerializeField] Image qSkillPanel;
    

    [Header("W스킬")]
    [SerializeField] float wBarrier;
    [SerializeField] float wTimer;
    [SerializeField] float wExlosiveWidth;
    [SerializeField] float wTimerMax = 8f;
    [SerializeField] float wBoomMinTime = 3f;
    [SerializeField] bool wSkillOn = false;
    [SerializeField] float wExlosionFixedDamage = 140f;
    [SerializeField] float wExlosionAddDamage = 0.4f;
    [SerializeField] float wExlosionAddEnemyHealth = 0.14f;
    [SerializeField] GameObject barrierPrefab;
    [SerializeField] Renderer barrierColor;
    [SerializeField] GameObject wExplosionPrefab;
    [SerializeField] Image wExplosionColor;
    Color wExplosiveOriginalColor;
    [Header("E스킬")]
    [SerializeField] GameObject eSkillPrefab;
    [SerializeField] float eSkillDamage;
    [SerializeField] float eSkillSlowPercent;
    [SerializeField] float eSkillArmorDownPercent;
    [SerializeField] float eSkillSpeed;
    [SerializeField] float eSkillTimer;
    GameObject copyPrefab;
    SionE findEPrefab;
    bool checkingEPrefab = false; 
    [Header("R스킬")]
    [SerializeField] float rSkillFixedMinDamage = 700;
    [SerializeField] float rSkillFixedMaxDamage = 1400;
    [SerializeField] float rSkillAddMinDamage = 0.4f;
    [SerializeField] float rSkillAddMaxDamage = 0.8f;
    [SerializeField] float rSkillSlowPercent = 0.75f;
    [SerializeField] float rSkillAirborneTime = 0.2f;
    [SerializeField] float rSkillMinStun = 0.75f;
    [SerializeField] float rSkillMaxStun = 1.75f;
    [SerializeField] bool rSkillOn = false;
    [SerializeField] float rSkillRotationSpeed;
    [SerializeField] float rSkillTimer;
    [SerializeField] float rSkillCheckDistance;
    [SerializeField] int rSkillIncreasedSpeed;
    [SerializeField] int rSkillIncreasing = 10;
    [SerializeField] float rSkillExpolosionWidth = 5;
    [SerializeField] float rSkillAirborneWidth = 2;
    [SerializeField] float rSkillCurStun;
    [SerializeField] float rSkillCurDamage;
    [SerializeField] float rSkillSlowTime;
    [SerializeField] Transform rSkillDestination;
    [SerializeField] Image rSkillExplosiveImage;
    Color rSkillOriginalColor;
    //[SerializeField] RectTransform rSkillCursor;


    PlayerController enemy;
    float baseAttackDamage;
    int lethal;
    float armorPenetration;
    Ray ray;
    RaycastHit hit;
   //bool canAA;


    //[SerializeField]Character character;
    //[SerializeField] BoxCollider qSkillcol;
    
    Dictionary<string, PlayerController> characterDictionary = new Dictionary<string, PlayerController>();
    public override void Start()
    {
        base.Start();
        //Red Blue tag 찾고
        //characterDictionary.Add("Blue", charcter);
        //qSkillcol = GetComponent<BoxCollider>();
        //qSkillOriginalCenterPos = qSkillcol.center;
        anim = GetComponentInChildren<Animator>();
        //playerController = GetComponent<PlayerController>();
        //agent = GetComponent<NavMeshAgent>();
        qSkillOriginalPanelA = qSkillPanel.color;
        hitLayer = 1 << LayerMask.NameToLayer(enemyTag);
        if(debuggingMode) PhotonNetwork.OfflineMode = true;//디버깅용
        if(barrierColor == null) barrierColor = barrierPrefab.GetComponent<Renderer>();

        rSkillOriginalColor = rSkillExplosiveImage.color;
        wExplosiveOriginalColor = wExplosionColor.color;


    }
    private void OnEnable()
    {
        canActivePassive = true;
    }
    [PunRPC]
    public override void Death()
    {
        if (canActivePassive)
        {
            canActivePassive = false;
            StartCoroutine(PassiveOn());
        }
        else
        {
            base.Death();
        }
    }
    public override void AutoAttack(PlayerController target)
    {
        if(!qSkillCharging && !rSkillOn)
        {
            
            Vector3 look = target.transform.position;
            look.y = transform.position.y;
            Debug.Log("평타" + target.name);
            float damage = character.ATK;

            if(character.CritChance >= Random.Range(0, 1f))//치명타
            {
                damage = character.CritDamage;
                anim.SetTrigger("Critical");
            }
            else if (isPassiveNow)
            {
                int x = 1;
                if(x == 1)
                {
                    x++;
                }
                else
                {
                    x--;
                }
                anim.SetTrigger("Attack" + x);
            }
            else
            {
                int animationNum = Random.Range(1, 4);
                anim.SetTrigger("Attack" + animationNum);
            }
            enemy = target;
            baseAttackDamage = damage;
           
            

        }
    }

    public void AttackResult()
    {
        if(enemy == null)
        {
            return;
        }
        if (isPassiveNow)
        {
            enemy.character.TakeDamage(character, baseAttackDamage + (enemy.character.HP * 0.1f), false, character.Lethality, character.ArmorPenetration);
        }
        else
        {
            enemy.character.TakeDamage(character, baseAttackDamage, false, character.Lethality, character.ArmorPenetration);
        }
        StartCoroutine(AAHandle());



    }

    // Update is called once per frame
    public override void Update()
    {

        if (pv != null &&pv.IsMine)
        {
            if ((character.CurState == State.Stun || character.CurState == State.Airborne || qSkillCharging || rSkillOn) == false)
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
                                pv.RPC("AAEnququer", RpcTarget.All, hit.transform.GetComponent<PhotonView>().ViewID);
                                //toExecute.Enqueue(new AutoAttackCommand(this, 0, hit.transform.GetComponent<PhotonView>().ViewID));
                            }
                        }
                        else
                        {
                            //toExecute.Enqueue(new MoveCommand(this, 0, hit.point));
                            Move(hit.point);
                        }
                    }
                }

                if ((isPassiveNow || character.CurState == State.Stun || character.CurState == State.Airborne) == false)
                {
                    if (Input.GetKeyDown(KeyCode.Q) && !rSkillOn && !qSkillCharging && character.CurQCool <= 0)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            SkillQ(false, true, null, hit.point);
                            PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
                            pv.RPC("SkillEnqueuer", RpcTarget.All, 1, qDelay, false, false, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
                            
                            //SkillEnqueuer(1, 0.2f, false, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point);
                            //toExecute.Enqueue(new SkillQCommand(this, 0.2f, false, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point));
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.W) && character.CurWCool <= 0)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            SkillW(false, false, null, transform.eulerAngles);
                            PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
                            pv.RPC("SkillEnqueuer", RpcTarget.All, 2, wDelay, false, false, enemyTemp != null ? enemyTemp.ViewID : 0, transform.eulerAngles);
                            //SkillEnqueuer(2, 0.1f, false, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point);
                            //toExecute.Enqueue(new SkillWCommand(this, 0.1f, true, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point));
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.E) && !rSkillOn && !qSkillCharging && character.CurECool <= 0)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            SkillE(false, false, null, hit.point);
                            PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
                            pv.RPC("SkillEnqueuer", RpcTarget.All, 3, eDelay, false, false, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
                            //SkillEnqueuer(3, 0.1f, false, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point);
                            //toExecute.Enqueue(new SkillECommand(this, 0.1f, true, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point));
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.R) && !rSkillOn && !qSkillCharging && character.CurRCool <= 0)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            SkillR(false, false, null, hit.point);
                            PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
                            pv.RPC("SkillEnqueuer", RpcTarget.All, 4, rDelay, false, false, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
                            //SkillEnqueuer(4, 0, false, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point);
                            //toExecute.Enqueue(new SkillRCommand(this, 0, false, false, hit.transform.GetComponent<PhotonView>().ViewID, hit.point));
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.D) && !rSkillOn && !qSkillCharging)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            toExecute.Enqueue(new RushCommand(this, 0));
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.F) && !rSkillOn && !qSkillCharging)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            toExecute.Enqueue(new FlashCommmand(this, 0, hit.point));
                        }
                    }
                  
                    
                }
                else if(isPassiveNow == true || (character.CurState == State.Stun || character.CurState == State.Airborne) == false)//여기가 패시브 상태
                {
                    if(Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.R) && !usedPassiveSkill)
                    {
                        StartCoroutine(PassiveSkill());
                    }
                }
               
                if (Input.GetKeyDown(KeyCode.S) && !rSkillOn && !qSkillCharging)
                {
                    Stop();
                    toExecute.Clear();
                }
            }
            else if (character.CurState == State.Root)
            {
                agent.ResetPath();
            }
            if(anim !=  null && !rSkillOn)
            {
                bool walking = HasReachedDestination();
                anim.SetBool("Walking", !walking);
                if(attackSpeedAnimFloat != 1 + (character.AttackSpeed * 0.2f))
                {
                    attackSpeedAnimFloat = 1 + (character.AttackSpeed * 0.2f);
                    attackSppedAnimGetFloat = anim.GetFloat("AttackSpeed");
                }
                
                if(attackSppedAnimGetFloat != attackSpeedAnimFloat)
                {
                    anim.SetFloat("AttackSpeed", attackSpeedAnimFloat);
                }

                if(moveSpeedAnimFloat != 1 +  character.MoveSpeed * 0.0005f)
                {
                    moveSpeedAnimFloat = 1 + (character.MoveSpeed * 0.0005f);
                    moveSpeedAnimGetFloat = anim.GetFloat("MoveSpeed");
                }
                if(moveSpeedAnimGetFloat != moveSpeedAnimFloat)
                {
                    anim.SetFloat("MoveSpeed", moveSpeedAnimFloat);
                }
                if(character.CurState == State.Slow)
                {
                    anim.SetBool("Slow", true);
                }
                else
                {
                    if (anim.GetBool("Slow"))
                    {
                        anim.SetBool("Slow", false);
                    }
                }
            }
            else if(anim == null)
            {
                Debug.Log("애니메이터 없는데");
            }
            if (rSkillOn)
            {

                RSkillCheckingHit();
                
                //Vector3 screenPosition = Input.mousePosition;
                //screenPosition.z = 5f; // UI가 카메라에서 일정 거리 유지
                //rSkillCursor.position = Camera.main.ScreenToWorldPoint(screenPosition);
                //Vector3 cursorRotation = new Vector3(0 ,0, -transform.eulerAngles.y);
                //rSkillCursor.eulerAngles = cursorRotation;

                
            }


        }

    }
    bool HasReachedDestination()
    {
        if (agent != null)
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }
        else
        {
            Debug.Log("Agent가 없다");
            return false;
        }
    }
    [PunRPC]
    IEnumerator PassiveOn()
    {
        agent.SetDestination(transform.position);
        character.SetState(State.Invincible ,2);
        character.SetState(State.Stun ,2);
        anim.Play("passiveDeath");
        isPassiveNow = true;
        originalAtkSpeed = character.AttackSpeed;
        originalHPRegen = character.HpRegen;
        float x = 1.75f - character.AttackSpeed;
 
        yield return new WaitForSeconds(2f);
        character.ResetState();
        character.Heal(character.HP);
        if(x < 0)
        {
            character.AdjustAtkSpeed(x);
        }
        else
        {
            character.AdjustAtkSpeed(-x);
        }
        
       
        character.AdjustHPRegen(-character.HpRegen);

        while (isPassiveNow && character.CurHP > 0)
        {
            character.TakeDamage(character, -(1.3f + (passiveCount * 0.7f)), true, 0, 0);
            passiveCount++;
            yield return passiveDecreaseHPTime;
        }
        character.AdjustHPRegen(originalHPRegen);
        character.AdjustAtkSpeed(-1.75f);
        character.AdjustAtkSpeed(originalAtkSpeed);
        anim.Play("SionDeath");
        base.Death();

    }
    [PunRPC]
    IEnumerator PassiveSkill()
    {
        usedPassiveSkill = true;
        int increasedSpeed = (int)(character.MoveSpeed * 0.5f);
        int baseSpeed = character.MoveSpeed;
        int targetSpeed = increasedSpeed + baseSpeed;
        character.AdjustMoveSpeed(targetSpeed);
        while(passiveTime < 1.5f)
        {
            float t = passiveTime / 1.5f;
            int currentSpeed = (int)Mathf.Lerp(targetSpeed, baseSpeed, t);
            character.AdjustMoveSpeed(-currentSpeed);

            passiveTime += Time.deltaTime;
            yield return null;
        }
    }

    public override void SkillQ(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if(!qSkillCharging && !rSkillOn)
        {
            agent.SetDestination(transform.position);
            GetMouseCursorPos();
            StartCoroutine(QSkillCharge());
        }
    }
    [PunRPC]
    IEnumerator QSkillCharge()
    {
        qSkillTimer = 0;
        qSkillCharging = true;
        
        anim.SetTrigger("Q");
        qSkillPanel.color = qSkillOriginalPanelA;
        while (Input.GetKey(KeyCode.Q) && qSkillTimer <= 2f && character.CurHP > 0)
        {
            PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
            pv.RPC("SkillEnqeuer", RpcTarget.All, 1, qDelay, false, true, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
            if(character.CurState == State.Stun || character.CurState == State.Airborne)
            {
                qSkillPanel.gameObject.SetActive(false);
                qSKillImage.fillAmount = 0;
                character.SetQCooldown();
                character.CurQCool = 2f;
                yield break;
            }
            chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
            qSkillTimer += Time.deltaTime;
            qSKillImage.fillAmount = chargeRatio;
            if(qSkillTimer >= 1.2)
            {
                if(!qSkillPanel.gameObject.activeSelf) qSkillPanel.gameObject.SetActive(true);
                Color a = qSkillOriginalPanelA;
                a.a = Mathf.Lerp(qSkillPanel.color.a, qSkillMaxAlphaFloat, Time.deltaTime * 2);
                qSkillPanel.color = a;
            }
            yield return null;
        }
        
        CastQSkill();
        qSkillCharging = false;

        #region 콜라이더로 하려다가 말았다
        //qSkillcol.center = qSkillOriginalCenterPos;
        //while(qSkillCharging == true)
        //{
        //    qSkillTimer += Time.deltaTime;
        //    float chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
        //    qSkillcol.size = new Vector3(qSkillcol.size.x, qSkillcol.size.y, Mathf.Lerp(qSkillMinSize, qSkillMaxSize, chargeRatio));
        //    qSkillcol.center = new Vector3(qSkillcol.center.x, qSkillcol.center.y, Mathf.Lerp(qSkillcol.size.z,qSkillcol.size.z + qSkillMaxSize, chargeRatio));
        //    qSkillCurDamage = Mathf.Lerp(qSkillMinFixedDamage + qSkillMinAddDamage, qSkillMaxFixedDamage + qSkillMaxAddDamage, qSkillTimer);
        //    
        //    yield return null;
        //}
        //
        //if(qSkillCharging == false)//이 부분은 소통상 함수에 들어가야할 수도 있음 ㅇㅇ
        //{
        //    if(qSkillTimer >= 1f)
        //    {
        //        //대충 에어본 쌉가능
        //        //기절도 드가시고 ㅋ
        //    }
        //    else
        //    {
        //        //대충 슬로우 넣는다 ㅇㅇ
        //    }
        //}
        //
        //qSkillcol.enabled = true;
        #endregion
    }
    [PunRPC]
    void CastQSkill()
    {
        qSkillPanel.gameObject.SetActive(false);
        skillLength = Mathf.Lerp(qSkillMinLength, qSkillMaxLength, chargeRatio);
        Vector3 boxCenter = transform.position + transform.forward * (skillLength / 2f);
        Vector3 halfExtents = new Vector3(qSkillWidth / 2f, 0.5f, skillLength / 2f);
        Quaternion boxOrientation = transform.rotation;
        qSkillCurDamage = Mathf.Lerp(qSkillMinFixedDamage, qSkillMaxFixedDamage, qSkillTimer) + Mathf.Lerp(qSkillMinAddDamage, qSkillMaxAddDamage, qSkillChargeTime);
        
        if(qSkillTimer >= 1)
        {
            qSkillCurAirborne = Mathf.Lerp(qSkillAirborneTimeMin, qSkillAirborneTimeMax, chargeRatio / 2f);
            qSkillCurStun = Mathf.Lerp(qSkillStunTimeMix, qSkillStunTimeMax, qSkillTimer / 2f) + qSkillCurAirborne;
        }
        Collider[] hits = Physics.OverlapBox(boxCenter, halfExtents, boxOrientation, hitLayer);

        foreach(Collider hit in hits)
        {
            if (characterDictionary.TryGetValue(hit.tag, out PlayerController enemy) == true)
            {
                enemyRb = enemy.GetComponent<Rigidbody>();
                if (qSkillTimer >= 1)
                {
                    StartCoroutine(SettingEnemyState(enemy, State.Airborne, qSkillCurAirborne, true, 0));
                    StartCoroutine(ApplyAirborne(enemyRb, qSkillCurAirborne));
                    StartCoroutine(SettingEnemyState(enemy, State.Stun, qSkillCurStun, false, 0));
                    
                }
                else
                {
                    StartCoroutine(SettingEnemyState(enemy, State.Slow, 0.5f, false, qSkillSlowPercentage));
                }
                

            }
            
            Debug.Log(hit.tag);
        }

        if (qSkillTimer >= 1)
        {
            Debug.Log("Full charging Q!");
            anim.SetTrigger("QLong");
        }
        else
        {
            Debug.Log("짧게 쓰기");
            anim.SetTrigger("QShort");
        }

        qSKillImage.fillAmount = 0;
        anim.ResetTrigger("WBoom");
        character.SetQCooldown();
    }

    public override void SkillW(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if (!wSkillOn)
        {
            wBarrier = (character.HP * 0.16f) + 120 + (character.ATK * 0.4f);
            character.AdjustBarrier(wBarrier);
            wSkillOn = true;
            StartCoroutine(CastWSkill());
        }
        
    }
    [PunRPC]
    IEnumerator CastWSkill()
    {
        wTimer = 0;
        Color x = barrierColor.material.color;
        x.a = 0.4f;
        barrierColor.material.color = x;
        PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
        pv.RPC("SkillEnqeuer", RpcTarget.All, 2, wDelay, false, true, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
        barrierPrefab.SetActive(true);
        while(wSkillOn == true && wTimer <= wTimerMax && character.Barrier > 0)
        {
            wTimer += Time.deltaTime;
            barrierPrefab.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            Color color = x;
            if(color.a < 222)
            {
                color.a = Mathf.Lerp(barrierColor.material.color.a, 0.8f, wTimer);
            }
            barrierColor.material.color = color;
            
            if(wTimer >= 3 && Input.GetKeyDown(KeyCode.W))
            {
                WSkillExplosion();
                barrierPrefab.SetActive(false);
            }
            yield return null;
        }
        if(wSkillOn == true && wTimer >= wTimerMax && character.Barrier > 0)
        {
            WSkillExplosion();
            character.SetWCooldown();
            barrierPrefab.SetActive(false);
            yield break;
        }
        else
        {
            wSkillOn = false;
            barrierPrefab.SetActive(false);
            character.SetWCooldown();
            yield break;
        }
    }
    [PunRPC]
    public void WSkillExplosion()
    {
        PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
       
        wSkillOn = false;
        pv.RPC("SkillEnqeuer", RpcTarget.All, 2, wDelay, false, true, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
        Collider[] hits = Physics.OverlapSphere(transform.position, wExlosiveWidth, hitLayer);
        wExplosionPrefab.transform.position = transform.position;
        wExplosionColor.color = wExplosiveOriginalColor;
        wExplosionPrefab.SetActive(true);
        StartCoroutine(WExplosionFade());
        foreach(Collider enemy in hits)
        {
            PlayerController x = enemy.GetComponent<PlayerController>();
            x.character.TakeDamage(character, wExlosionFixedDamage + (character.ATK * wExlosionAddDamage) + (x.character.HP * wExlosionAddEnemyHealth), false, character.Lethality, character.ArmorPenetration);
            Debug.Log(enemy);
        }
        anim.SetTrigger("WBoom");


    }
    [PunRPC]
    IEnumerator WExplosionFade()
    {
        float x = 0f;
        Color c = wExplosiveOriginalColor;
        Vector3 fixedPosition = wExplosionPrefab.transform.position;
        PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
        
        while (4f > x)
        {
            pv.RPC("SkillEnqeuer", RpcTarget.All, 2, wDelay, false, true, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);
            wExplosionPrefab.transform.position = fixedPosition;
            wExplosionPrefab.transform.localRotation = Quaternion.Euler(0f,0f,0f);   

            x += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, x);
            wExplosionColor.color = c;
            yield return null;
        }
       
            wExplosionPrefab.SetActive(false);
        
    }


    public override void SkillE(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {

        
        if (qSkillCharging && rSkillOn) return;
        GetMouseCursorPos();
        if (!checkingEPrefab)
        {
            findEPrefab = FindObjectOfType<SionE>();
            if (findEPrefab == null)
            {
                eSkillPrefab = Instantiate(eSkillPrefab);
            }
            checkingEPrefab = true;
        }
        eSkillPrefab.gameObject.SetActive(true);
        StartCoroutine(CastESkill());
        character.SetECooldown();
        anim.SetTrigger("E");
    }
    [PunRPC]
    IEnumerator CastESkill()
    {
        eSkillPrefab.transform.position = transform.position;
        eSkillTimer = 0;
        Collider[] hits = Physics.OverlapSphere(eSkillPrefab.transform.position, 1, hitLayer);
        Vector3 targetPos = transform.forward;
        while (eSkillPrefab.gameObject.activeSelf && eSkillTimer < 0.1f && hits.Length == 0)
        {
            eSkillTimer += Time.deltaTime;
            eSkillPrefab.transform.Translate((targetPos * eSkillSpeed) * Time.deltaTime);
            hits = Physics.OverlapSphere(eSkillPrefab.transform.position, 1, hitLayer);

            yield return skillCheckTime;
        }
        if (hits.Length > 0)
        {
            var x = hits[0].GetComponent<PlayerController>();
            x.character.TakeDamage(character, eSkillDamage + (character.ATK * 0.55f), false, character.Lethality, character.ArmorPenetration);
            StartCoroutine(SettingEnemyState(x, State.Slow, 2.5f, false, (int)(character.MoveSpeed * 0.6f)));
            StartCoroutine(ESkillArmorDown(x));
        }
       
        eSkillPrefab.SetActive(false);
        PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
        pv.RPC("SkillEnqeuer", RpcTarget.All, 2, wDelay, false, true, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);

    }
    [PunRPC]
    IEnumerator ESkillArmorDown(PlayerController enemy)
    {
        float x = enemy.character.DEF * 0.2f;
        enemy.character.AdjustDef(-x);
        yield return new WaitForSeconds(4f);
        enemy.character.AdjustDef(x);
    }
    public override void SkillR(bool isTargeting, bool isChanneling, PlayerController target, Vector3 location)
    {
        if(qSkillCharging == false && !rSkillOn)
        {
            GetMouseCursorPos();
            rSkillOn = true;
            StartCoroutine(CastRSkill());
        }
        else
        {
            return;
        }
    }
    [PunRPC]
    IEnumerator CastRSkill()
    {
        character.SetState(State.Unstoppable,8);
        anim.SetTrigger("R");
        anim.SetBool("RRunnig",false);
 
        while (rSkillOn == true && rSkillTimer < 8 && character.CurHP > 0)
        {
            Debug.Log(rSkillTimer);
            rSkillTimer += 0.05f;
            // 사이온의 현재 위치 + 전방 방향으로 박스 중심 설정
            Vector3 boxCenter = transform.position + transform.forward * 0.5f; // 적절한 거리 조정

            // 박스 크기 설정
            Vector3 halfExtents = new Vector3(0.8f, 1.5f, 0.8f); //반절 크기를 사용하니 반절

            // 박스 방향 설정
            Quaternion boxRotation = transform.rotation;

            // OverlapBox 실행
            Collider[] hits = Physics.OverlapBox(boxCenter, halfExtents, boxRotation, hitLayer);

            RSkillRotation();
           

            if (hits.Length > 0)
            {
                anim.SetTrigger("RHit");
                agent.SetDestination(transform.position);
                RSkillExplosion();
            }
           
            if(rSkillTimer >= 2 && anim.GetBool("RRunning") == false)
            {
                anim.SetBool("RRunning", true);
                anim.SetFloat("MoveSpeed", 2f);
            }


            
            if(character.MoveSpeed <= 950)
            {
                character.AdjustMoveSpeed(rSkillIncreasing);
                rSkillIncreasedSpeed += rSkillIncreasing;
               
            }
            Debug.Log("궁 코루틴");
            Move(rSkillDestination.position);
            if (Input.GetKey(KeyCode.R) && rSkillTimer > 0.3f)
            {
                anim.SetTrigger("RStop");
            }
            
            yield return skillCheckTime;
        }
        if(rSkillTimer >= 8)
        {
            anim.SetTrigger("RStop");
        }

        

    }


    [PunRPC]
    void RSkillCheckingHit()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit wallHit, rSkillCheckDistance, wallLayer))
        {
            Debug.Log("벽에 박았당");
            rSkillOn = false;
            anim.SetTrigger("RHit");
            agent.SetDestination(transform.position);
            RSkillExplosion();
            StartCoroutine(SettingEnemyState(this, State.Stun, 0.2f, false, 0));
        }
    }
    [PunRPC]
    void RSkillRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDirection = (hit.point - transform.position).normalized;
            targetDirection.y = 0; // Y축 회전 방지(수평 회전만 적용)

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rSkillRotationSpeed * Time.deltaTime);
        }
    }
    [PunRPC]
    public void RSkillExplosion()
    {
        rSkillExplosiveImage.color = rSkillOriginalColor;
        rSkillExplosiveImage.transform.position = transform.position;
        rSkillExplosiveImage.gameObject.SetActive(true);
        StartCoroutine(RSKillExplosiveFade());
        Collider[] targets = Physics.OverlapSphere(transform.position, rSkillExpolosionWidth, hitLayer);
        Collider[] airborneTargets = Physics.OverlapSphere(transform.position, rSkillAirborneWidth, hitLayer);
        rSkillCurDamage = Mathf.Lerp(rSkillFixedMinDamage +(character.ATK * rSkillAddMinDamage), rSkillFixedMaxDamage +( character.ATK * rSkillAddMaxDamage), rSkillTimer - 4);
        Debug.Log("빵야");
        rSkillOn = false;
        anim.ResetTrigger("RStop");

        character.AdjustMoveSpeed(-rSkillIncreasedSpeed);

        if (targets.Length > 0 )
        {
            foreach(Collider collider in targets)
            {
                PlayerController target = collider.GetComponent<PlayerController>();
                
                target.character.TakeDamage(character,rSkillCurDamage, false, character.Lethality, character.ArmorPenetration);
                int x = (int)(target.character.MoveSpeed * rSkillSlowPercent);
                StartCoroutine(SettingEnemyState(target, State.Slow, 1f, false, x));
            }
        }
        if(airborneTargets.Length > 0)
        {
            rSkillCurStun = Mathf.Lerp(rSkillMinStun, rSkillMaxStun, rSkillTimer - 3);   
            foreach(Collider col in airborneTargets)
            {
                enemyRb = col.GetComponent<Rigidbody>();
                PlayerController airborneTarget = col.GetComponent<PlayerController>();
                StartCoroutine(SettingEnemyState(airborneTarget, State.Airborne, rSkillAirborneTime, true, 0));
                StartCoroutine(ApplyAirborne(enemyRb, rSkillAirborneTime));
            }
        }
        character.ResetState();

        rSkillTimer = 0;
        rSkillIncreasedSpeed = 0;
        rSkillCurDamage = 0;
        rSkillCurStun = 0;
    }
    [PunRPC]
    IEnumerator RSKillExplosiveFade()
    {
        PhotonView enemyTemp = hit.transform.GetComponent<PhotonView>();
        float x = 0;
        Vector3 fixedPosition = rSkillExplosiveImage.transform.position;
        Color c = rSkillOriginalColor;
        while(3 > x)
        {
           
            pv.RPC("SkillEnqeuer", RpcTarget.All, 2, wDelay, false, true, enemyTemp != null ? enemyTemp.ViewID : 0, hit.point);

            rSkillExplosiveImage.transform.position = fixedPosition;
            rSkillExplosiveImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            x += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, x);
            rSkillExplosiveImage.color = c;
            yield return null;
        }
        rSkillExplosiveImage.gameObject.SetActive(false);
    }


    /// <summary>
    /// 적에게 부여할 상태이상과 그 시간을 작성하여 넣을 것, 다중이면 True 하고 마지막 CC에 False 넣을 것
    /// 슬로우는 마지막에 넣으셈 없다면 0
    /// </summary>
    /// <param name="state"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    [PunRPC]
    IEnumerator SettingEnemyState(PlayerController target ,State state, float time, bool multipleCC, int slow)
    {
        if(slow != 0)
        {
            target.character.AdjustMoveSpeed(-slow);
        }
        target.character.SetState(state, time);
        Debug.Log($"상태이상 적용 대상: {target}, 종류: {state}, 시간: {time}"); 
        yield return new WaitForSeconds(time);
        if (multipleCC == true)
        {
            yield break;
        }
        else
        {
            target.character.AdjustMoveSpeed(slow);
          
        }
        
    }
    [PunRPC]
    IEnumerator ApplyAirborne(Rigidbody rb, float totalAirborneTime)
    {
        float airborneTime = totalAirborneTime / 2; // 떠 있는 시간 (절반)
        float fallTime = totalAirborneTime / 2;     // 내려오는 시간 (절반)

        //띄우기
        rb.velocity = Vector3.zero; // 기존 속도 초기화
        rb.AddForce(Vector3.up * powerOfAirborne, ForceMode.Impulse); // 위로
        yield return new WaitForSeconds(airborneTime);

        //착지
        float elapsedTime = 0f;
        while (elapsedTime < fallTime)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.down * 3f, elapsedTime / fallTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //최종적으로 땅에 닿으면 속도 초기화
        rb.velocity = Vector3.zero;
    }

    void GetMouseCursorPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookDir = (hit.point - transform.position).normalized;
            lookDir.y = 0; // 높이 변화 무시 (수평 방향만 고려)
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }


   
    void OnDrawGizmos()
    {
        if (!debuggingMode) return;

        //비율 0~1
        float chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
        //스킬 넓이
        float skillLength = Mathf.Lerp(qSkillMinLength, qSkillMaxLength, chargeRatio);

        Vector3 boxCenter = transform.position + transform.forward * (skillLength / 2f);
        Vector3 halfExtents = new Vector3(qSkillWidth / 2f, 0.5f, skillLength / 2f);
        Quaternion boxOrientation = transform.rotation;

        Gizmos.color = Color.green;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxOrientation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
            Gizmos.matrix = oldMatrix;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wExlosiveWidth);

        Vector3 rSkillBoxCenter = transform.position + transform.forward * 0.5f;
        Vector3 rSkillhalfExtents = new Vector3(0.8f, 1.5f, 0.8f);
        Quaternion boxRotation = transform.rotation;

        Gizmos.color = Color.red;
        Matrix4x4 rSkilloldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(rSkillBoxCenter, boxRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, rSkillhalfExtents * 2); // 원래 크기로 표시
        Gizmos.matrix = rSkilloldMatrix;

    }


}
   