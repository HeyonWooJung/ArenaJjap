using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SionSkill : PlayerController
{
    Animator anim;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] bool debuggingMode;
    readonly WaitForSeconds skillCheckTime = new WaitForSeconds(0.05f);
    //[SerializeField] PlayerController playerController;
    [SerializeField] GameObject skillCanvas;
    new NavMeshAgent agent;
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
    [SerializeField] float qSkillSlowPercentage = 75f;
    //[SerializeField] bool qSkillAirborne;
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
    //[SerializeField] Vector3 qSkillOriginalCenterPos;

    [Header("W스킬")]
    [SerializeField] float wBarrier;
    [SerializeField] float wTimer;
    [SerializeField] float wExlosiveWidth;
    [SerializeField] float wTimerMax = 8f;
    [SerializeField] float wBoomMinTime = 3f;
    [SerializeField] bool wSkillOn = false;
    [Header("E스킬")]
    [SerializeField] GameObject eSkillPrefab;
    [SerializeField] float eSkillDamage;
    [SerializeField] float eSkillSlowPercent;
    [SerializeField] float eSkillArmorDownPercent;
    [SerializeField] float eSkillSpeed;
    [SerializeField] float eSkillTimer;
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



    PlayerController enemy;
    float baseAttackDamage;
    int lethal;
    float armorPenetration;
   


    //[SerializeField]Character character;
    //[SerializeField] BoxCollider qSkillcol;
    
    Dictionary<string, PlayerController> characterDictionary = new Dictionary<string, PlayerController>();
    void Start()
    {
        //Red Blue tag 찾고
        //characterDictionary.Add("Blue", charcter);
        //qSkillcol = GetComponent<BoxCollider>();
        //qSkillOriginalCenterPos = qSkillcol.center;
        anim = GetComponentInChildren<Animator>();
        //playerController = GetComponent<PlayerController>();
        agent = GetComponentInParent<NavMeshAgent>();
        qSkillOriginalPanelA = qSkillPanel.color;
        hitLayer = 1 << LayerMask.NameToLayer(enemyTag);

    }

    public override void AutoAttack(PlayerController target)
    {
        if(!qSkillCharging && !rSkillOn)
        {
            StartCoroutine(AAHandle());
            Vector3 look = target.transform.position;
            look.y = transform.position.y;
            Debug.Log("평타" + target.name);
            float damage = character.ATK;

            if(character.CritChance >= Random.Range(0, 1f))//치명타
            {
                damage = character.CritDamage;
                anim.SetTrigger("Critical");
            }
            else
            {
                int animationNum = Random.Range(1, 4);
                anim.SetTrigger("Attack" + animationNum);
            }
            enemy = target;
            baseAttackDamage = damage;
            lethal = character.Lethality;
            armorPenetration = character.ArmorPenetration;
            

        }
    }

    public void AttackResult()
    {
        if(enemy == null)
        {
            return;
        }

        enemy.character.TakeDamage(baseAttackDamage, false, lethal, armorPenetration);

    }

    // Update is called once per frame
    void Update()
    {
       
       
        
        if(Input.GetKeyDown(KeyCode.R) && qSkillCharging == false && !rSkillOn)
        {
            if (!rSkillOn)
            {
                RSkill();
                GetMouseCursorPos();
            }
            
        }
        if (rSkillOn)
        {
            RSkillCheckingHit();
            RSkillRotation();
            rSkillTimer += Time.deltaTime;
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

    IEnumerator QSkillCharge()
    {
        qSkillTimer = 0;
        qSkillCharging = true;
        
        anim.SetTrigger("Q");
        qSkillPanel.color = qSkillOriginalPanelA;
        while (Input.GetKey(KeyCode.Q) && qSkillTimer <= 2f)
        {
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

    void CastQSkill()
    {
        //비율 0~1
        //chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
        //스킬 넓이

        qSkillPanel.gameObject.SetActive(false);
        skillLength = Mathf.Lerp(qSkillMinLength, qSkillMaxLength, chargeRatio);

        Vector3 boxCenter = transform.position + transform.forward * (skillLength / 2f);
        Vector3 halfExtents = new Vector3(qSkillWidth / 2f, 0.5f, skillLength / 2f);
        Quaternion boxOrientation = transform.rotation;
        


        //if(CC걸림 == true) 밑에 실행 X
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
                if (qSkillTimer >= 1)
                {
                    //에어본 + 기절


                    //enemy.SetState(State.Airborne);
                    //enemy.SetState(State.Stun);
                   
                }
                
            }
            else
            {
                    //enemy.SetState(State.Slow);
            }
            Debug.Log(hit.tag);
            //enemy.TakeDamage(qSkillCurDamage, false, character.Lethality, character.ArmorPenetration);
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
        character.SetQCooldown();
    }

    public void WSkill()
    {
        if (!wSkillOn)
        {
            wBarrier = (character.HP * 0.16f) + 120 + (character.ATK * 0.4f);
            character.AdjustBarrier(wBarrier);
            wSkillOn = true;
            StartCoroutine(CastWSkill());
        }
        
    }

    IEnumerator CastWSkill()
    {
        wTimer = 0;
        while(wSkillOn == true && wTimer <= wTimerMax && character.Barrier > 0)
        {
            wTimer += Time.deltaTime;
            if(wTimer >= 3 && Input.GetKeyDown(KeyCode.W))
            {
                WSkillExplosion();
            }
            yield return null;
        }
        if(wSkillOn == true && wTimer >= wTimerMax && character.Barrier > 0)
        {
            WSkillExplosion();
            character.SetWCooldown();
            yield break;
        }
        else
        {
            wSkillOn = false;
            character.SetWCooldown();
            yield break;
        }
    }

    public void WSkillExplosion()
    {
        wSkillOn = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, wExlosiveWidth, hitLayer);
        foreach(Collider enemy in hits)
        {
            if(characterDictionary.TryGetValue(enemy.tag, out PlayerController x))
            {
                //Debug.Log(x.name);
            }
            Debug.Log(enemy);
        }
        anim.SetTrigger("WBoom");


    }


    public void ESkill()
    {
        if (!checkingEPrefab)
        {
            findEPrefab = FindObjectOfType<SionE>();
            if (findEPrefab == null)
            {
                eSkillPrefab = Instantiate(eSkillPrefab);
            }
            checkingEPrefab = true;
        }
        if (eSkillPrefab.activeSelf) return;
        if (qSkillCharging && rSkillOn) return;
        GetMouseCursorPos();
        eSkillPrefab.gameObject.SetActive(true);
        StartCoroutine(CastESkill());
        character.SetECooldown();
    }

    IEnumerator CastESkill()
    {
        eSkillPrefab.transform.position = transform.position;
        eSkillTimer = 0;
        Collider[] hits = Physics.OverlapSphere(eSkillPrefab.transform.position, 1, hitLayer);
        Vector3 targetPos = transform.forward;
        while (eSkillPrefab.gameObject.activeSelf && eSkillTimer < 0.7f)
        {
            eSkillTimer += Time.deltaTime;
            eSkillPrefab.transform.Translate((targetPos * eSkillSpeed) * Time.deltaTime);
            hits = Physics.OverlapSphere(eSkillPrefab.transform.position, 1, hitLayer);
            yield return skillCheckTime;
        }
        eSkillPrefab.SetActive(false);
        //대충 계속한다는 말
        //if(hits != null)
        //{
        //    hits[0].tag
        //}
    }

    public void RSkill()
    {
        if(qSkillCharging == false && !rSkillOn)
        {
            rSkillOn = true;
            StartCoroutine(CastRSkill());
        }
        else
        {
            return;
        }
    }

    IEnumerator CastRSkill()
    {
        anim.SetTrigger("R");
        rSkillTimer = 0;
        rSkillIncreasedSpeed = 0;
        rSkillCurDamage = 0;
        rSkillCurStun = 0;

        while (rSkillOn == true && rSkillTimer < 8)
        {
            
            // 사이온의 현재 위치 + 전방 방향으로 박스 중심 설정
            Vector3 boxCenter = transform.position + transform.forward * 0.5f; // 적절한 거리 조정

            // 박스 크기 설정
            Vector3 halfExtents = new Vector3(0.8f, 1.5f, 0.8f); //반절 크기를 사용하니 반절

            // 박스 방향 설정
            Quaternion boxRotation = transform.rotation;

            // OverlapBox 실행
            Collider[] hits = Physics.OverlapBox(boxCenter, halfExtents, boxRotation, hitLayer);
            
            if (hits.Length > 0)
            {
                foreach (Collider hit in hits)
                {
                    Debug.Log($"R 스킬 적중: {hit.gameObject.name}");
                    
                }
                rSkillOn = false;
                if (anim.GetBool("RRunning") == true)
                {
                    anim.SetBool("RRunning", false);
                }
                anim.SetTrigger("RHit");
                character.AdjustMoveSpeed(-rSkillIncreasedSpeed);
                agent.SetDestination(transform.position);
            }
           
            if(rSkillTimer >= 2 && anim.GetBool("RRunning") == false)
            {
                anim.SetBool("RRunning", true);
            }

            
            if(character.MoveSpeed <= 950)
            {
                character.AdjustMoveSpeed(rSkillIncreasing);
                rSkillIncreasedSpeed += rSkillIncreasing;
               
            }

            if(rSkillTimer >= 1 && Input.GetKeyDown(KeyCode.R))
            {
                anim.SetTrigger("RStop");

            }

            //playerController.Move(skillCanvas.transform.position);
            //Move(skillCanvas.transform.position);
            yield return skillCheckTime;
        }
        
    }

    void RSkillCheckingHit()
    {
        if(Physics.Raycast(transform.position, transform.forward, rSkillCheckDistance, wallLayer))
        {
            Debug.Log("벽에 박았당");
            rSkillOn = false;
            if(anim.GetBool("RRunning") == true)
            {
                anim.SetBool("RRunning",false);
            }
            anim.SetTrigger("RHit");
            character.AdjustMoveSpeed(-rSkillIncreasedSpeed);
            agent.SetDestination(transform.position);

        }
    }
    void RSkillRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 targetDirection = (hit.point - transform.position).normalized;
            targetDirection.y = 0; // Y축 회전 방지(수평 회전만 적용)

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rSkillRotationSpeed * Time.deltaTime);
        }
    }

    public void RSkillExplosion()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, rSkillExpolosionWidth, hitLayer);
        Collider[] airborneTargets = Physics.OverlapSphere(transform.position, rSkillAirborneWidth, hitLayer);
        rSkillCurDamage = Mathf.Lerp(rSkillFixedMinDamage + rSkillAddMinDamage, rSkillFixedMaxDamage + rSkillAddMaxDamage, rSkillTimer - 3);
        

        if(targets.Length > 0 )
        {
            foreach(Collider collider in targets)
            {
                PlayerController target = collider.GetComponent<PlayerController>();
                target.character.TakeDamage(rSkillCurDamage, false, character.Lethality, character.ArmorPenetration);
                int x = (int)(target.character.MoveSpeed * rSkillSlowPercent);
                StartCoroutine(SettingEnemyState(target, State.Slow, 1f, false, x));
            }
        }
        if(airborneTargets.Length > 0)
        {
            rSkillCurStun = Mathf.Lerp(rSkillMinStun, rSkillMaxStun, rSkillTimer - 3);   
            foreach(Collider col in airborneTargets)
            {
                PlayerController airborneTarget = col.GetComponent<PlayerController>();
                StartCoroutine(SettingEnemyState(airborneTarget, State.Airborne, rSkillAirborneTime, true, 0));
                StartCoroutine(SettingEnemyState(airborneTarget, State.Airborne, rSkillCurStun, false, 0));
            }
        }

    }

    /// <summary>
    /// 적에게 부여할 상태이상과 그 시간을 작성하여 넣을 것, 다중이면 True 하고 마지막 CC에 False 넣을 것
    /// 슬로우는 마지막에 넣으셈 없다면 0
    /// </summary>
    /// <param name="state"></param>
    /// <param name="time"></param>
    /// <returns></returns>

    IEnumerator SettingEnemyState(PlayerController target ,State state, float time, bool multipleCC, int slow)
    {
        if(slow != 0)
        {
            target.character.AdjustMoveSpeed(-slow);
        }
        target.character.SetState(state);
        yield return new WaitForSeconds(time);
        if (multipleCC == true)
        {
            yield break;
        }
        else
        {
            target.character.AdjustMoveSpeed(slow);
            target.character.ResetState();
        }
        
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
   