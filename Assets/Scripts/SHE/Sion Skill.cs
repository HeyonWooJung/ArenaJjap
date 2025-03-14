using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SionSkill : MonoBehaviour
{
    Animator anim;
    
    [SerializeField] float skillQDamage = 0;
    [SerializeField] float skillQAddDamage = 2.4f;
    [SerializeField] float qSkillChargeTime = 2f;
    [SerializeField] float qSkillAirbornTimeMin = 0.3f;
    [SerializeField] float qSkillAirbornTimeMax = 0.6f;
    [SerializeField] float qSkillStunTimeMix = 1f;
    [SerializeField] float qSkillStunTimeMax = 1.75f;
    float maxChargeTime = 2f;
    bool qSkillCharging = false;

    


    [SerializeField]Character charcter;
    [SerializeField] BoxCollider qSkillcol;
    
    Dictionary<string, Character> characterDictionary = new Dictionary<string, Character>();
    void Start()
    {
        //Red Blue tag 찾고
        characterDictionary.Add("Blue", charcter);
        qSkillcol = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void QSkill()
    {
        
    }

    IEnumerator QSkillCharge()
    {
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Q)) ;
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
