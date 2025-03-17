using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SionSkill : MonoBehaviour
{
    Animator anim;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] bool debuggingMode;
 
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
    //[SerializeField] Vector3 qSkillOriginalCenterPos;
   

    [SerializeField] float wBarrier;
    [SerializeField] float wTimer;
    [SerializeField] float wTimerMax = 8f;
    [SerializeField] float wBoomMinTime = 3f;



    [SerializeField]Character character;
    //[SerializeField] BoxCollider qSkillcol;
    
    Dictionary<string, Character> characterDictionary = new Dictionary<string, Character>();
    void Start()
    {
        //Red Blue tag 찾고
        //characterDictionary.Add("Blue", charcter);
        //qSkillcol = GetComponent<BoxCollider>();
        //qSkillOriginalCenterPos = qSkillcol.center;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            QSkill();
        }
    }

    public void UpdateEnemyDictionary()
    {
        //Red Blue tag찾아요
        if(this.gameObject.tag == "Blue1" || this.gameObject.tag == "Blue2")
        {
            characterDictionary.Clear();
            characterDictionary.Add("Red1", character);
            characterDictionary.Add("Red2", character);
        }
        else
        {
            characterDictionary.Clear();
            characterDictionary.Add("Blue1", character);
            characterDictionary.Add("Blue2", character);
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
        float chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
        //스킬 넓이
        float skillLength = Mathf.Lerp(qSkillMinLength, qSkillMaxLength, chargeRatio);

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
            if (characterDictionary.TryGetValue(hit.tag, out Character enemy) == true)
            {
                if (qSkillTimer >= 1)
                {
                    //에어본 + 기절


                    enemy.SetState(State.Airborne);
                    enemy.SetState(State.Stun);
                   
                }
                
            }
            else
            {
                    enemy.SetState(State.Slow);
            }
            Debug.Log(hit.tag);
            enemy.TakeDamage(qSkillCurDamage, false, character.Lethality, character.ArmorPenetration);
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


    //QWR 적중시 어쩌구 해주세요 감사합니다 ^^
    private void OnTriggerEnter(Collider other)
    {
         



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
        
    }


}
   