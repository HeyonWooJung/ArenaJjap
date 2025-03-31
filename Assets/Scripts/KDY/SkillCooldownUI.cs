using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public PlayerController targetController;

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

    [Header("D Slot (Rush)")]
    public Image dFillImage;
    public Text dCooldownText;
    public float dCooldownDuration = 240f;
    private float dLastUsedTime = -999f;

    [Header("F Slot (Flash)")]
    public Image fFillImage;
    public Text fCooldownText;
    public float fCooldownDuration = 300f;
    private float fLastUsedTime = -999f;

    [Header("HP UI")]
    public Image hpFillImage;

    [Header("HP Text (Split)")]
    public Text hpLeftText;   // 현재 체력
    public Text hpRightText;  // 최대 체력

    //  최대 체력 캐싱용 변수
    private float cachedMaxHP;

    void Start()
    {
        // 최초 최대 체력 캐싱
        if (targetController != null && targetController.character != null)
        {
            cachedMaxHP = targetController.character.HP;
        }
    }

    void Update()
    {
        if (targetController == null || targetController.character == null)
        {
            Debug.LogWarning("targetController 또는 character가 연결되지 않았습니다.");
            return;
        }

        UpdateSkillUI(targetController.character.CurQCool, GetSkillMaxCooldown("qCoolDown"), qFillImage, qCooldownText, "Q");
        UpdateSkillUI(targetController.character.CurWCool, GetSkillMaxCooldown("wCoolDown"), wFillImage, wCooldownText, "W");
        UpdateSkillUI(targetController.character.CurECool, GetSkillMaxCooldown("eCoolDown"), eFillImage, eCooldownText, "E");
        UpdateSkillUI(targetController.character.CurRCool, GetSkillMaxCooldown("rCoolDown"), rFillImage, rCooldownText, "R");

        UpdateDFCooldown("D", targetController.character.CanRush, dFillImage, dCooldownText, dCooldownDuration, ref dLastUsedTime);
        UpdateDFCooldown("F", targetController.character.CanFlash, fFillImage, fCooldownText, fCooldownDuration, ref fLastUsedTime);

        UpdateHPUI(); // 체력 UI 갱신

        ////  테스트용: 스페이스바 누르면 체력 200 깎기
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    targetController.character.AdjustHP(-500);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 캐릭터에 데미지를 주는 방식으로 체력 감소
            targetController.character.TakeDamage(200, true, 0, 0f);
        }


    }

    void UpdateSkillUI(float currentCool, float maxCool, Image fillImage, Text cooldownText, string label)
    {
        float ratio = Mathf.Clamp01(currentCool / maxCool);

        if (fillImage != null)
            fillImage.fillAmount = ratio;

        if (cooldownText != null)
        {
            cooldownText.text = currentCool > 0 ? Mathf.CeilToInt(currentCool).ToString() : "";
        }
    }

    void UpdateDFCooldown(string label, bool isAvailable, Image fillImage, Text cooldownText, float cooldownDuration, ref float lastUsedTime)
    {
        if (isAvailable)
        {
            if (fillImage != null) fillImage.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";
            lastUsedTime = Time.time;
        }
        else
        {
            float elapsed = Time.time - lastUsedTime;
            float remain = Mathf.Clamp(cooldownDuration - elapsed, 0f, cooldownDuration);
            float ratio = remain / cooldownDuration;

            if (fillImage != null) fillImage.fillAmount = ratio;
            if (cooldownText != null) cooldownText.text = Mathf.CeilToInt(remain).ToString();
        }
    }

    //  HP UI 갱신 (현재 체력만 변함, 최대 체력은 고정)
    void UpdateHPUI()
    {
        float curHP = Mathf.Max(0f, targetController.character.CurHP);

        if (hpFillImage != null)
            hpFillImage.fillAmount = Mathf.Clamp01(curHP / cachedMaxHP);

        if (hpLeftText != null)
            hpLeftText.text = $"{(int)curHP}";

        if (hpRightText != null)
            hpRightText.text = $"/ {(int)cachedMaxHP}";
    }

    float GetSkillMaxCooldown(string fieldName)
    {
        var field = targetController.character.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (float)field.GetValue(targetController.character);
        }
        else
        {
            Debug.LogWarning($"쿨타임 필드 {fieldName} 를 찾을 수 없습니다.");
            return 1f;
        }
    }
}
