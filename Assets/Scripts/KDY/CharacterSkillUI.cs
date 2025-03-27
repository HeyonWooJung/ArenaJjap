//using UnityEngine;

///// <summary>
///// 모든 캐릭터가 공유할 수 있는 스킬 UI 쿨타임 제어 스크립트
///// 스킬이 실제로 사용되었을 때, 해당 슬롯에 쿨타임을 시작시킵니다.
///// 각 캐릭터의 컨트롤러에서 필요할 때 이 스크립트를 참조해 호출합니다.
///// </summary>
//public class CharacterSkillUI : MonoBehaviour
//{
//    // 슬롯별 SkillSlotUI 연결 (필요한 슬롯만 연결해도 됩니다)
//    public SkillSlotUI qSlot;
//    public SkillSlotUI wSlot;
//    public SkillSlotUI eSlot;
//    public SkillSlotUI rSlot;
//    public SkillSlotUI dSlot;
//    public SkillSlotUI fSlot;

//    PlayerController playerController;

//    private void Start()
//    {
//        playerController = GetComponentInParent<PlayerController>();
//    }
//    // Q 스킬 쿨타임 시작
//    public void StartQCooldown(float cool)
//    {
//        cool = playerController.character.CurQCool;
//        qSlot.SetCooldown(cool);
//    }

//    // W 스킬 쿨타임 시작
//    public void StartWCooldown(float cool)
//    {
//        if (wSlot != null)
//            wSlot.SetCooldown(cool);
//    }

//    // E 스킬 쿨타임 시작
//    public void StartECooldown(float cool)
//    {
//        if (eSlot != null)
//            eSlot.SetCooldown(cool);
//    }

//    // R 궁극기 쿨타임 시작
//    public void StartRCooldown(float cool)
//    {
//        if (rSlot != null)
//            rSlot.SetCooldown(cool);
//    }

//    // D 키 (소환사 주문 등) 쿨타임 시작
//    public void StartDCooldown(float cool)
//    {
//        if (dSlot != null)
//            dSlot.SetCooldown(cool);
//    }

//    // F 키 (소환사 주문 등) 쿨타임 시작
//    public void StartFCooldown(float cool)
//    {
//        if (fSlot != null)
//            fSlot.SetCooldown(cool);
//    }
//}

using UnityEngine;

public class CharacterSkillUI : MonoBehaviour
{
    // 각 스킬 슬롯 UI 연결
    public SkillSlotUI qSlot;
    public SkillSlotUI wSlot;
    public SkillSlotUI eSlot;
    public SkillSlotUI rSlot;
    public SkillSlotUI dSlot;
    public SkillSlotUI fSlot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            qSlot.SetCooldown(12f); // Q 스킬 쿨 12초
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            wSlot.SetCooldown(8f); // W 스킬 쿨 8초
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            eSlot.SetCooldown(10f); // E 스킬 쿨 10초
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rSlot.SetCooldown(100f); // R 스킬 쿨 100초
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            dSlot.SetCooldown(300f); // 점멸 같은 소환사 주문
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fSlot.SetCooldown(240f); // 회복 등
        }
    }
}



