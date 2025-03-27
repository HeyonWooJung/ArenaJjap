//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//// ��ų ���� �ϳ�(Q, W, E, R, D, F �� �ϳ�)�� ��Ÿ�� UI�� ����ϴ� ��ũ��Ʈ
//// ��Ÿ���� ���۵Ǹ� �̹��� ä��� ȿ���� ���� �ؽ�Ʈ�� �����ְ�
//// ��Ÿ���� ������ �ڵ����� ����
//public class SkillSlotUI : MonoBehaviour
//{
//    // ��Ÿ�� ���� ��Ȳ�� �ð������� ������ �̹��� (Filled ���)
//    public Image fillImage;

//    // ���� ��Ÿ�� ���ڸ� ǥ���� �ؽ�Ʈ
//    public Text cooldownText;

//    // �ִ� ��Ÿ�� (ex. 12��)
//    private float maxCool;

//    // ���� ���� ��Ÿ�� (�ʴ� ���ҵ�)
//    private float curCool;

//    // ��ٿ��� ���� ������ ���� üũ��
//    private bool isCooling = false;

//    // ��ٿ� ������ �ܺο��� ���� Ȯ���� �� �ִ� ����
//    // ��: TryndamereSkillUI���� �� ������ üũ�� �� ���
//    // false�� �� �ƴ�(��� ����), true�� �� ��(��� �Ұ�)
//    public bool isCoolingNow = false;


//    void Start()
//    {
//        //������ �� �̹����� �ؽ�Ʈ ��� ��Ȱ��ȭ
//        fillImage.fillAmount = 0f;
//        cooldownText.text = "";
//    }


//    // �� �� �Լ��� �ܺο��� ��Ÿ���� ���۵� �� ȣ���ؾ� ��
//    // �� ��: ��ų�� ����� ����, �ش� ���Կ� ��Ÿ���� ���۽�Ű�� ����
//    public void SetCooldown(float newMaxCool)
//    {
//        if (isCoolingNow) return;  // �̹� �� ���̸� ����

//        maxCool = newMaxCool;     // �ִ� ��Ÿ�� ����
//        curCool = newMaxCool;     // ���� �� �ִ뿡�� ����
//        isCooling = true;         // ��Ÿ�� ���� ���·� ��ȯ
//        isCoolingNow = true;      // �� ���۵�

//        //fillImage.fillAmount = 1f;    // �̹��� �� ä��
//        cooldownText.text = Mathf.Ceil(curCool).ToString();  // ���� ǥ��
//    }

//    // �� �� �Լ��� �ܺο��� ��Ÿ���� ������ �� ��� (�ñر�, ���� ������)
//    public void SetMaxCool(float newMaxCool)
//    {
//        maxCool = newMaxCool; // �ִ� �𰪸� ���� (���� ���� ���� �� ���� X)
//    }

//    // �� �� �����Ӹ��� ȣ���. ��Ÿ���� ���� ���̸� ���� ���ҽ�Ű�� UI ����
//    void Update()
//    {
//        if (!isCooling) return; // ������ �ƴϸ� ���� �� ��

//        curCool -= Time.deltaTime; // �ð� ����

//        // fillImage.fillAmount = curCool / maxCool; // ä��� ���� ���̱�
//        fillImage.fillAmount = Mathf.Clamp01(maxCool/Time.deltaTime ); // ä��� ���� ���̱�
//        cooldownText.text = Mathf.Ceil(curCool).ToString(); // ���� �ð� �ؽ�Ʈ ����

//        // ��Ÿ���� �������� ����
//        if (curCool <= 0f)
//        {
//            isCooling = false;             // �� ����
//            isCoolingNow = false;          // �� ���� ǥ��

//            fillImage.fillAmount = 0f;     // �̹��� ���
//            cooldownText.text = "";        // �ؽ�Ʈ ����
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;

// ��ų ���� �ϳ�(Q, W, E, R, D, F �� �ϳ�)�� ��Ÿ�� UI�� ����ϴ� ��ũ��Ʈ
// �� ��ũ��Ʈ�� ��Ÿ���� ���۵Ǹ� �̹��� ä��� ȿ���� ���� �ؽ�Ʈ�� �����ְ�
// ��Ÿ���� ������ �ڵ����� ������ �մϴ�.
public class SkillSlotUI : MonoBehaviour
{
    // ��Ÿ�� ���� ��Ȳ�� �ð������� ������ �̹��� (Filled ���)
    public Image fillImage;

    // ���� ��Ÿ�� ���ڸ� ǥ���� �ؽ�Ʈ
    public Text cooldownText;

    // �ִ� ��Ÿ�� (ex. 12��)
    private float maxCool;

    // ���� ���� ��Ÿ�� (�ʴ� ���ҵ�)
    private float curCool;

    // ��ٿ��� ���� ������ ���� üũ��
    private bool isCooling = false;

    public bool isCoolingNow = false;

    void Start()
    {
        fillImage.fillAmount = 0f;
        cooldownText.text = "";
    }

    // �� �� �Լ��� �ܺο��� ��Ÿ���� ���۵� �� ȣ���ؾ� ��
    // �� ��: ��ų�� ����� ����, �ش� ���Կ� ��Ÿ���� ���۽�Ű�� ����
    public void SetCooldown(float newMaxCool)
    {
        if (isCoolingNow) return;  //�̹� �� ���̸� ����

        maxCool = newMaxCool;     // �ִ� ��Ÿ�� ����
        curCool = newMaxCool;     // ���� �� �ִ뿡�� ����
        isCooling = true;         // ��Ÿ�� ���� ���·� ��ȯ
        isCoolingNow = true;   //  �� ���۵�

        fillImage.fillAmount = 1f;    // �̹��� �� ä��
        cooldownText.text = Mathf.Ceil(curCool).ToString();  // ���� ǥ��
    }

    // �� �� �Լ��� �ܺο��� ��Ÿ���� ������ �� ��� (�ñر�, ���� ������)
    public void SetMaxCool(float newMaxCool)
    {
        maxCool = newMaxCool; // �ִ� �𰪸� ���� (���� ���� ���� �� ���� X)
    }

    // �� �� �����Ӹ��� ȣ���. ��Ÿ���� ���� ���̸� ���� ���ҽ�Ű�� UI ����
    void Update()
    {
        if (!isCooling) return; // ������ �ƴϸ� ���� �� ��

        curCool -= Time.deltaTime; // �ð� ����

        fillImage.fillAmount = curCool / maxCool; // ä��� ���� ���̱�
        cooldownText.text = Mathf.Ceil(curCool).ToString(); // ���� �ð� �ؽ�Ʈ ����

        // ��Ÿ���� �������� ����
        if (curCool <= 0f)
        {
            isCooling = false;             // �� ����
            isCoolingNow = false;
            fillImage.fillAmount = 0f;     // �̹��� ���
            cooldownText.text = "";        // �ؽ�Ʈ ����
        }
    }
}



