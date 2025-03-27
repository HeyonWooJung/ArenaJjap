//using UnityEngine;

///// <summary>
///// ��� ĳ���Ͱ� ������ �� �ִ� ��ų UI ��Ÿ�� ���� ��ũ��Ʈ
///// ��ų�� ������ ���Ǿ��� ��, �ش� ���Կ� ��Ÿ���� ���۽�ŵ�ϴ�.
///// �� ĳ������ ��Ʈ�ѷ����� �ʿ��� �� �� ��ũ��Ʈ�� ������ ȣ���մϴ�.
///// </summary>
//public class CharacterSkillUI : MonoBehaviour
//{
//    // ���Ժ� SkillSlotUI ���� (�ʿ��� ���Ը� �����ص� �˴ϴ�)
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
//    // Q ��ų ��Ÿ�� ����
//    public void StartQCooldown(float cool)
//    {
//        cool = playerController.character.CurQCool;
//        qSlot.SetCooldown(cool);
//    }

//    // W ��ų ��Ÿ�� ����
//    public void StartWCooldown(float cool)
//    {
//        if (wSlot != null)
//            wSlot.SetCooldown(cool);
//    }

//    // E ��ų ��Ÿ�� ����
//    public void StartECooldown(float cool)
//    {
//        if (eSlot != null)
//            eSlot.SetCooldown(cool);
//    }

//    // R �ñر� ��Ÿ�� ����
//    public void StartRCooldown(float cool)
//    {
//        if (rSlot != null)
//            rSlot.SetCooldown(cool);
//    }

//    // D Ű (��ȯ�� �ֹ� ��) ��Ÿ�� ����
//    public void StartDCooldown(float cool)
//    {
//        if (dSlot != null)
//            dSlot.SetCooldown(cool);
//    }

//    // F Ű (��ȯ�� �ֹ� ��) ��Ÿ�� ����
//    public void StartFCooldown(float cool)
//    {
//        if (fSlot != null)
//            fSlot.SetCooldown(cool);
//    }
//}

using UnityEngine;

public class CharacterSkillUI : MonoBehaviour
{
    // �� ��ų ���� UI ����
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
            qSlot.SetCooldown(12f); // Q ��ų �� 12��
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            wSlot.SetCooldown(8f); // W ��ų �� 8��
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            eSlot.SetCooldown(10f); // E ��ų �� 10��
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rSlot.SetCooldown(100f); // R ��ų �� 100��
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            dSlot.SetCooldown(300f); // ���� ���� ��ȯ�� �ֹ�
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fSlot.SetCooldown(240f); // ȸ�� ��
        }
    }
}



