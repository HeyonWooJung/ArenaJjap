//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//// 스킬 슬롯 하나(Q, W, E, R, D, F 중 하나)의 쿨타임 UI를 담당하는 스크립트
//// 쿨타임이 시작되면 이미지 채우기 효과와 숫자 텍스트를 보여주고
//// 쿨타임이 끝나면 자동으로 꺼짐
//public class SkillSlotUI : MonoBehaviour
//{
//    // 쿨타임 진행 상황을 시각적으로 보여줄 이미지 (Filled 방식)
//    public Image fillImage;

//    // 남은 쿨타임 숫자를 표시할 텍스트
//    public Text cooldownText;

//    // 최대 쿨타임 (ex. 12초)
//    private float maxCool;

//    // 현재 남은 쿨타임 (초당 감소됨)
//    private float curCool;

//    // 쿨다운이 진행 중인지 여부 체크용
//    private bool isCooling = false;

//    // 쿨다운 중인지 외부에서 쉽게 확인할 수 있는 변수
//    // 예: TryndamereSkillUI에서 쿨 중인지 체크할 때 사용
//    // false면 쿨 아님(사용 가능), true면 쿨 중(사용 불가)
//    public bool isCoolingNow = false;


//    void Start()
//    {
//        //시작할 때 이미지와 텍스트 모두 비활성화
//        fillImage.fillAmount = 0f;
//        cooldownText.text = "";
//    }


//    // ▶ 이 함수는 외부에서 쿨타임이 시작될 때 호출해야 함
//    // ▶ 예: 스킬을 사용한 순간, 해당 슬롯에 쿨타임을 시작시키기 위해
//    public void SetCooldown(float newMaxCool)
//    {
//        if (isCoolingNow) return;  // 이미 쿨 중이면 무시

//        maxCool = newMaxCool;     // 최대 쿨타임 설정
//        curCool = newMaxCool;     // 현재 쿨도 최대에서 시작
//        isCooling = true;         // 쿨타임 시작 상태로 전환
//        isCoolingNow = true;      // 쿨 시작됨

//        //fillImage.fillAmount = 1f;    // 이미지 꽉 채움
//        cooldownText.text = Mathf.Ceil(curCool).ToString();  // 숫자 표시
//    }

//    // ▶ 이 함수는 외부에서 쿨타임을 변경할 때 사용 (궁극기, 증강 등으로)
//    public void SetMaxCool(float newMaxCool)
//    {
//        maxCool = newMaxCool; // 최대 쿨값만 수정 (현재 진행 중인 쿨엔 영향 X)
//    }

//    // ▶ 매 프레임마다 호출됨. 쿨타임이 진행 중이면 값을 감소시키고 UI 갱신
//    void Update()
//    {
//        if (!isCooling) return; // 쿨중이 아니면 실행 안 함

//        curCool -= Time.deltaTime; // 시간 감소

//        // fillImage.fillAmount = curCool / maxCool; // 채우기 비율 줄이기
//        fillImage.fillAmount = Mathf.Clamp01(maxCool/Time.deltaTime ); // 채우기 비율 줄이기
//        cooldownText.text = Mathf.Ceil(curCool).ToString(); // 남은 시간 텍스트 갱신

//        // 쿨타임이 끝났으면 정리
//        if (curCool <= 0f)
//        {
//            isCooling = false;             // 쿨 종료
//            isCoolingNow = false;          // 쿨 종료 표시

//            fillImage.fillAmount = 0f;     // 이미지 비움
//            cooldownText.text = "";        // 텍스트 숨김
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;

// 스킬 슬롯 하나(Q, W, E, R, D, F 중 하나)의 쿨타임 UI를 담당하는 스크립트
// 이 스크립트는 쿨타임이 시작되면 이미지 채우기 효과와 숫자 텍스트를 보여주고
// 쿨타임이 끝나면 자동으로 꺼지게 합니다.
public class SkillSlotUI : MonoBehaviour
{
    // 쿨타임 진행 상황을 시각적으로 보여줄 이미지 (Filled 방식)
    public Image fillImage;

    // 남은 쿨타임 숫자를 표시할 텍스트
    public Text cooldownText;

    // 최대 쿨타임 (ex. 12초)
    private float maxCool;

    // 현재 남은 쿨타임 (초당 감소됨)
    private float curCool;

    // 쿨다운이 진행 중인지 여부 체크용
    private bool isCooling = false;

    public bool isCoolingNow = false;

    void Start()
    {
        fillImage.fillAmount = 0f;
        cooldownText.text = "";
    }

    // ▶ 이 함수는 외부에서 쿨타임이 시작될 때 호출해야 함
    // ▶ 예: 스킬을 사용한 순간, 해당 슬롯에 쿨타임을 시작시키기 위해
    public void SetCooldown(float newMaxCool)
    {
        if (isCoolingNow) return;  //이미 쿨 중이면 무시

        maxCool = newMaxCool;     // 최대 쿨타임 설정
        curCool = newMaxCool;     // 현재 쿨도 최대에서 시작
        isCooling = true;         // 쿨타임 시작 상태로 전환
        isCoolingNow = true;   //  쿨 시작됨

        fillImage.fillAmount = 1f;    // 이미지 꽉 채움
        cooldownText.text = Mathf.Ceil(curCool).ToString();  // 숫자 표시
    }

    // ▶ 이 함수는 외부에서 쿨타임을 변경할 때 사용 (궁극기, 증강 등으로)
    public void SetMaxCool(float newMaxCool)
    {
        maxCool = newMaxCool; // 최대 쿨값만 수정 (현재 진행 중인 쿨엔 영향 X)
    }

    // ▶ 매 프레임마다 호출됨. 쿨타임이 진행 중이면 값을 감소시키고 UI 갱신
    void Update()
    {
        if (!isCooling) return; // 쿨중이 아니면 실행 안 함

        curCool -= Time.deltaTime; // 시간 감소

        fillImage.fillAmount = curCool / maxCool; // 채우기 비율 줄이기
        cooldownText.text = Mathf.Ceil(curCool).ToString(); // 남은 시간 텍스트 갱신

        // 쿨타임이 끝났으면 정리
        if (curCool <= 0f)
        {
            isCooling = false;             // 쿨 종료
            isCoolingNow = false;
            fillImage.fillAmount = 0f;     // 이미지 비움
            cooldownText.text = "";        // 텍스트 숨김
        }
    }
}



