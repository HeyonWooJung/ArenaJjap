using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SionSkill : MonoBehaviour
{
    Animator anim;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] bool debuggingMode;
    readonly WaitForSeconds skillCheckTime = new WaitForSeconds(0.05f);
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject skillCanvas;
    [Header("Q��ų")]
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
    [SerializeField] float qSkillMinLength;
    [SerializeField] float qSkillMaxLength;
    [SerializeField] float qSkillWidth;
    float chargeRatio;
    float skillLength;
    //[SerializeField] Vector3 qSkillOriginalCenterPos;

    [Header("W��ų")]
    [SerializeField] float wBarrier;
    [SerializeField] float wTimer;
    [SerializeField] float wExlosiveWidth;
    [SerializeField] float wTimerMax = 8f;
    [SerializeField] float wBoomMinTime = 3f;
    [SerializeField] bool wSkillOn = false;
    [Header("E��ų")]
    [SerializeField] GameObject eSkillPrefab;
    [SerializeField] float eSkillDamage;
    [SerializeField] float eSkillSlowPercent;
    [SerializeField] float eSkillArmorDownPercent;
    [SerializeField] float eSkillSpeed;
    [SerializeField] float eSkillTimer;
    [Header("R��ų")]
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




    [SerializeField]Character character;
    //[SerializeField] BoxCollider qSkillcol;
    
    Dictionary<string, PlayerController> characterDictionary = new Dictionary<string, PlayerController>();
    void Start()
    {
        //Red Blue tag ã��
        //characterDictionary.Add("Blue", charcter);
        //qSkillcol = GetComponent<BoxCollider>();
        //qSkillOriginalCenterPos = qSkillcol.center;
        anim = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            QSkill();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            WSkill();
        }
        if( Input.GetKeyDown(KeyCode.E))
        {
            ESkill();
        }
        if(Input.GetKeyDown(KeyCode.R))
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

    public void UpdateEnemyDictionary()
    {
        //Red Blue tagã�ƿ�
        if(this.gameObject.tag == "Blue1" || this.gameObject.tag == "Blue2")
        {
            characterDictionary.Clear();
            
        }
        else
        {
            characterDictionary.Clear();

        }
    }

    

    public void QSkill()
    {
        if(!qSkillCharging)
        {
            GetMouseCursorPos();
            StartCoroutine(QSkillCharge());
        }
       
        
    }

    IEnumerator QSkillCharge()
    {
        qSkillTimer = 0;
        qSkillCharging = true;
        
        anim.SetTrigger("Q");

        while(Input.GetKey(KeyCode.Q) && qSkillTimer <= 2f)
        {
            qSkillTimer += Time.deltaTime;
            yield return null;
        }

        CastQSkill();
        qSkillCharging = false;

        #region �ݶ��̴��� �Ϸ��ٰ� ���Ҵ�
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
        //if(qSkillCharging == false)//�� �κ��� ����� �Լ��� ������ ���� ���� ����
        //{
        //    if(qSkillTimer >= 1f)
        //    {
        //        //���� ��� �԰���
        //        //������ �尡�ð� ��
        //    }
        //    else
        //    {
        //        //���� ���ο� �ִ´� ����
        //    }
        //}
        //
        //qSkillcol.enabled = true;
        #endregion
    }

    void CastQSkill()
    {
        //���� 0~1
        chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
        //��ų ����
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
                if (qSkillTimer >= 1)
                {
                    //��� + ����


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
            Debug.Log("ª�� ����");
            anim.SetTrigger("QShort");
        }



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
            yield break;
        }
        else
        {
            wSkillOn = false;
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
        if (eSkillPrefab.activeSelf) return;
        GetMouseCursorPos();
        eSkillPrefab.gameObject.SetActive(true);
        StartCoroutine(CastESkill());
    }

    IEnumerator CastESkill()
    {
        eSkillPrefab.transform.position = transform.position;
        eSkillTimer = 0;
        Collider[] hits = Physics.OverlapSphere(eSkillPrefab.transform.position, 1, hitLayer);
        while (eSkillPrefab.gameObject.activeSelf && eSkillTimer < 0.7f)
        {
            eSkillTimer += Time.deltaTime;
            eSkillPrefab.transform.Translate((transform.forward * eSkillSpeed) * Time.deltaTime);
            hits = Physics.OverlapSphere(eSkillPrefab.transform.position, 1, hitLayer);
            yield return skillCheckTime;
        }
        eSkillPrefab.SetActive(false);
        //���� ����Ѵٴ� ��
        //if(hits != null)
        //{
        //    hits[0].tag
        //}
    }

    public void RSkill()
    {
        rSkillOn = true;
        StartCoroutine(CastRSkill());
        
    }

    IEnumerator CastRSkill()
    {
        rSkillTimer = 0;
        while(rSkillOn == true || rSkillTimer < 8)
        {
            
            // ���̿��� ���� ��ġ + ���� �������� �ڽ� �߽� ����
            Vector3 boxCenter = transform.position + transform.forward * 0.5f; // ������ �Ÿ� ����

            // �ڽ� ũ�� ����
            Vector3 halfExtents = new Vector3(0.8f, 1.5f, 0.8f); //���� ũ�⸦ ����ϴ� ����

            // �ڽ� ���� ����
            Quaternion boxRotation = transform.rotation;

            // OverlapBox ����
            Collider[] hits = Physics.OverlapBox(boxCenter, halfExtents, boxRotation, hitLayer);
            anim.SetTrigger("R");
            if (hits.Length > 0)
            {
                foreach (Collider hit in hits)
                {
                    Debug.Log($"R ��ų ����: {hit.gameObject.name}");
                    
                }
                rSkillOn = false;
                if (anim.GetBool("RRunning") == true)
                {
                    anim.SetBool("RRunning", false);
                }
                anim.SetTrigger("RHit");

            }
           
            if(rSkillTimer >= 2 && anim.GetBool("RRunning") == false)
            {
                anim.SetBool("RRunning", true);
            }

            int increaseMoveSpeed = 20;
            if(character.MoveSpeed <= 950)
            {
                character.AdjustMoveSpeed(increaseMoveSpeed);
               
            }
            playerController.Move(skillCanvas.transform.position);
            yield return skillCheckTime;
        }
    }

    void RSkillCheckingHit()
    {
        if(Physics.Raycast(transform.position, transform.forward, rSkillCheckDistance, wallLayer))
        {
            Debug.Log("���� �ھҴ�");
            rSkillOn = false;
            if(anim.GetBool("RRunning") == true)
            {
                anim.SetBool("RRunning",false);
            }
            anim.SetTrigger("RHit");
        }
    }
    void RSkillRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 targetDirection = (hit.point - transform.position).normalized;
            targetDirection.y = 0; // Y�� ȸ�� ����(���� ȸ���� ����)

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rSkillRotationSpeed * Time.deltaTime);
        }
    }



    void GetMouseCursorPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookDir = (hit.point - transform.position).normalized;
            lookDir.y = 0; // ���� ��ȭ ���� (���� ���⸸ ���)
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }


    //QWR ���߽� ��¼�� ���ּ��� �����մϴ� ^^
    private void OnTriggerEnter(Collider other)
    {
         



    }

    void OnDrawGizmos()
    {
        if (!debuggingMode) return;

        //���� 0~1
        float chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
        //��ų ����
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
        Gizmos.DrawWireCube(Vector3.zero, rSkillhalfExtents * 2); // ���� ũ��� ǥ��
        Gizmos.matrix = rSkilloldMatrix;

    }


}
   