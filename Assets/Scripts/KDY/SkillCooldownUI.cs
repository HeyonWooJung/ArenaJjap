using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    // ���� ���� ���� ĳ���� (PlayerController) ����
    public PlayerController targetController;

    // QWER ��ų UI (Fill �̹��� + �ؽ�Ʈ)
    [Header("Q Slot")]
    public Image qFillImage;
    public Text qCooldownText;

    [Header("W Slot")]
    public Image wFillImage;
    public Text wCooldownText;

    [Header("E Slot")]
    public Image eFillImage;
    public Text eCooldownText;

    [Header("R Slot")]
    public Image rFillImage;
    public Text rCooldownText;

    // D (��üȭ) ���� - ���� ��Ÿ�� ����
    [Header("D Slot (Rush)")]
    public Image dFillImage;
    public Text dCooldownText;
    public float dCooldownDuration = 240f; // ��üȭ ��Ÿ�� (��)
    private float dLastUsedTime = -999f;   // ������ ��� �ð� ���

    // F (����) ���� - ���� ��Ÿ�� ����
    [Header("F Slot (Flash)")]
    public Image fFillImage;
    public Text fCooldownText;
    public float fCooldownDuration = 300f; // ���� ��Ÿ�� (��)
    private float fLastUsedTime = -999f;

    void Update()
    {
        if (targetController == null || targetController.character == null)
        {
            Debug.LogWarning("targetController �Ǵ� character�� ������� �ʾҽ��ϴ�.");
            return;
        }

        // QWER ��ų ��Ÿ�� Fill, �ؽ�Ʈ ǥ�� ó��
        UpdateSkillUI(targetController.character.CurQCool, GetSkillMaxCooldown("qCoolDown"), qFillImage, qCooldownText, "Q");
        UpdateSkillUI(targetController.character.CurWCool, GetSkillMaxCooldown("wCoolDown"), wFillImage, wCooldownText, "W");
        UpdateSkillUI(targetController.character.CurECool, GetSkillMaxCooldown("eCoolDown"), eFillImage, eCooldownText, "E");
        UpdateSkillUI(targetController.character.CurRCool, GetSkillMaxCooldown("rCoolDown"), rFillImage, rCooldownText, "R");

        // DF ��ų�� bool ���� ����̹Ƿ� ���� ó��
        UpdateDFCooldown("D", targetController.character.CanRush, dFillImage, dCooldownText, dCooldownDuration, ref dLastUsedTime);
        UpdateDFCooldown("F", targetController.character.CanFlash, fFillImage, fCooldownText, fCooldownDuration, ref fLastUsedTime);
    }

    // QWER ��ų UI ������Ʈ (���簪 / �ִ밪 �������� FillAmount ��� �� �ؽ�Ʈ ǥ��)
    void UpdateSkillUI(float currentCool, float maxCool, Image fillImage, Text cooldownText, string label)
    {
        float ratio = Mathf.Clamp01(currentCool / maxCool);

        // FillAmount ��ȭ ����
        if (fillImage != null)
        {
            if (Mathf.Abs(fillImage.fillAmount - ratio) > 0.001f)
            {
                Debug.Log($"[{label}] FillAmount ���� ����: {fillImage.fillAmount:0.00} �� {ratio:0.00}");
            }
            fillImage.fillAmount = ratio;
        }

        // �ؽ�Ʈ ��ȭ ����
        if (cooldownText != null)
        {
            if (currentCool > 0)
            {
                int time = Mathf.CeilToInt(currentCool);
                if (cooldownText.text != time.ToString())
                {
                    Debug.Log($"[{label}] �ؽ�Ʈ ǥ�� ����: {time}�� ����");
                }
                cooldownText.text = time.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(cooldownText.text))
                {
                    Debug.Log($"[{label}] ��Ÿ�� ���� �� �ؽ�Ʈ �����");
                }
                cooldownText.text = "";
            }
        }
    }

    // D / F ��ų ��Ÿ�� UI ó�� (bool ���� ��� ���� ����)
    void UpdateDFCooldown(string label, bool isAvailable, Image fillImage, Text cooldownText, float cooldownDuration, ref float lastUsedTime)
    {
        if (isAvailable)
        {
            // ��� ���� ������ ��� Fill 0, �ؽ�Ʈ ����
            if (fillImage != null && fillImage.fillAmount != 0f)
            {
                Debug.Log($"[{label}] ��� ���� ���� �� Fill 0, �ؽ�Ʈ �����");
            }

            if (cooldownText != null && !string.IsNullOrEmpty(cooldownText.text))
            {
                Debug.Log($"[{label}] ��� ���� ���� �� �ؽ�Ʈ �ʱ�ȭ");
            }

            if (fillImage != null) fillImage.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";

            // ��Ÿ�� �ʱ�ȭ
            lastUsedTime = Time.time;
        }
        else
        {
            float elapsed = Time.time - lastUsedTime;
            float remain = Mathf.Clamp(cooldownDuration - elapsed, 0f, cooldownDuration);
            float ratio = remain / cooldownDuration;

            if (fillImage != null && Mathf.Abs(fillImage.fillAmount - ratio) > 0.001f)
            {
                Debug.Log($"[{label}] FillAmount ���� ��: {fillImage.fillAmount:0.00} �� {ratio:0.00}");
            }

            if (cooldownText != null && cooldownText.text != Mathf.CeilToInt(remain).ToString())
            {
                Debug.Log($"[{label}] �ؽ�Ʈ ǥ�� ��: {Mathf.CeilToInt(remain)}�� ����");
            }

            if (fillImage != null) fillImage.fillAmount = ratio;
            if (cooldownText != null) cooldownText.text = Mathf.CeilToInt(remain).ToString();
        }
    }

    // Character.cs�� private ��Ÿ�� ��(qCoolDown ��)�� ���÷������� �о��
    float GetSkillMaxCooldown(string fieldName)
    {
        var field = targetController.character.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (float)field.GetValue(targetController.character);
        }
        else
        {
            Debug.LogWarning($"��Ÿ�� �ʵ� {fieldName} �� ã�� �� �����ϴ�.");
            return 1f;
        }
    }
}
