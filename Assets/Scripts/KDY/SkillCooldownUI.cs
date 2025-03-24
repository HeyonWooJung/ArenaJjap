using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("��Ÿ�� ����")]
    public KeyCode key;                // Q, W, E, R, D, F Ű
    public float cooldownTime = 5f;    // ��Ÿ�� (��)

    [Header("UI ����")]
    public Image fillImage;           // Filled �̹��� (Radial)
    public Text cooldownText;         // ��Ÿ�� �ؽ�Ʈ (Legacy UI Text)

    private bool isCoolingDown = false;

    void Start()
    {
        fillImage.fillAmount = 0f;
        fillImage.enabled = false;         //�������� ���� (SetActive X)
        cooldownText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && !isCoolingDown)
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private System.Collections.IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;
        float timer = cooldownTime;

        fillImage.fillAmount = 1f;
        fillImage.enabled = true;          // �ٽ� ���̰�
        cooldownText.text = Mathf.Ceil(timer).ToString("0");

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float ratio = Mathf.Clamp01(timer / cooldownTime);
            fillImage.fillAmount = ratio;
            cooldownText.text = Mathf.Ceil(timer).ToString("0");

            yield return null;
        }

        fillImage.fillAmount = 0f;
        fillImage.enabled = false;        // �ٽ� �����
        cooldownText.text = "";
        isCoolingDown = false;
    }
}
