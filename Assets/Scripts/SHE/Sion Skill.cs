using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SionSkill : MonoBehaviour
{
    Animator anim;
    
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
    float qSkillTimer;
    float maxChargeTime = 2f;
    bool qSkillCharging = false;
    [SerializeField] float qSkillMinSize;
    [SerializeField] float qSkillMaxSize;
    [SerializeField] Vector3 qSkillOriginalCenterPos;

    [SerializeField] float wBarrier;
    [SerializeField] float wTimer;
    [SerializeField] float wTimerMax = 8f;
    [SerializeField] float wBoomMinTime = 3f;



    [SerializeField]Character charcter;
    [SerializeField] BoxCollider qSkillcol;
    
    Dictionary<string, Character> characterDictionary = new Dictionary<string, Character>();
    void Start()
    {
        //Red Blue tag 찾고
        characterDictionary.Add("Blue", charcter);
        qSkillcol = GetComponent<BoxCollider>();
        qSkillOriginalCenterPos = qSkillcol.center;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void QSkill()
    {
        if(!qSkillCharging)
        {
            StartCoroutine(QSkillCharge());
        }
        
    }

    IEnumerator QSkillCharge()
    {
        qSkillTimer = 0;
        anim.SetTrigger("Q");
        while(qSkillCharging == true)
        {
            qSkillTimer += Time.deltaTime;
            float chargeRatio = Mathf.Clamp01(qSkillTimer / 1f);
            qSkillcol.size = new Vector3(qSkillcol.size.x, qSkillcol.size.y, Mathf.Lerp(qSkillMinSize, qSkillMaxSize, chargeRatio));
            qSkillcol.center = new Vector3(qSkillcol.center.x, qSkillcol.center.y, Mathf.Lerp(qSkillcol.size.z,qSkillcol.size.z + qSkillMaxSize, chargeRatio));
            qSkillCurDamage = Mathf.Lerp(qSkillMinFixedDamage + qSkillMinAddDamage, qSkillMaxFixedDamage + qSkillMaxAddDamage, qSkillTimer);
            
            yield return null;
        }
        
        if(qSkillCharging == false)//이 부분은 소통상 함수에 들어가야할 수도 있음 ㅇㅇ
        {
            if(qSkillTimer >= 1f)
            {
                //대충 에어본 쌉가능
                //기절도 드가시고 ㅋ
            }
            else
            {
                //대충 슬로우 넣는다 ㅇㅇ
            }
        }

        qSkillcol.enabled = true;
        
    }

    Vector3 GetMouseCursorPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return Vector3.zero;
    }


    //QWR 적중시 어쩌구 해주세요 감사합니다 ^^
    private void OnTriggerEnter(Collider other)
    {
         



    }


}
