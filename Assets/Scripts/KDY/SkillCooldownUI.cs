using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    // 현재 조작 중인 캐릭터 (PlayerController) 참조
    public PlayerController targetController;

    // QWER 스킬 UI (Fill 이미지 + 텍스트)
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

    // D (유체화) 슬롯 - 수동 쿨타임 추적
    [Header("D Slot (Rush)")]
    public Image dFillImage;
    public Text dCooldownText;
    public float dCooldownDuration = 240f; // 유체화 쿨타임 (초)
    private float dLastUsedTime = -999f;   // 마지막 사용 시간 기록

    // F (점멸) 슬롯 - 수동 쿨타임 추적
    [Header("F Slot (Flash)")]
    public Image fFillImage;
    public Text fCooldownText;
    public float fCooldownDuration = 300f; // 점멸 쿨타임 (초)
    private float fLastUsedTime = -999f;

    void Update()
    {
        if (targetController == null || targetController.character == null)
        {
            Debug.LogWarning("targetController 또는 character가 연결되지 않았습니다.");
            return;
        }

        // QWER 스킬 쿨타임 Fill, 텍스트 표시 처리
        UpdateSkillUI(targetController.character.CurQCool, GetSkillMaxCooldown("qCoolDown"), qFillImage, qCooldownText, "Q");
        UpdateSkillUI(targetController.character.CurWCool, GetSkillMaxCooldown("wCoolDown"), wFillImage, wCooldownText, "W");
        UpdateSkillUI(targetController.character.CurECool, GetSkillMaxCooldown("eCoolDown"), eFillImage, eCooldownText, "E");
        UpdateSkillUI(targetController.character.CurRCool, GetSkillMaxCooldown("rCoolDown"), rFillImage, rCooldownText, "R");

        // DF 스킬은 bool 상태 기반이므로 별도 처리
        UpdateDFCooldown("D", targetController.character.CanRush, dFillImage, dCooldownText, dCooldownDuration, ref dLastUsedTime);
        UpdateDFCooldown("F", targetController.character.CanFlash, fFillImage, fCooldownText, fCooldownDuration, ref fLastUsedTime);
    }

    // QWER 스킬 UI 업데이트 (현재값 / 최대값 기준으로 FillAmount 계산 및 텍스트 표시)
    void UpdateSkillUI(float currentCool, float maxCool, Image fillImage, Text cooldownText, string label)
    {
        float ratio = Mathf.Clamp01(currentCool / maxCool);

        // FillAmount 변화 감지
        if (fillImage != null)
        {
            if (Mathf.Abs(fillImage.fillAmount - ratio) > 0.001f)
            {
                Debug.Log($"[{label}] FillAmount 감소 시작: {fillImage.fillAmount:0.00} → {ratio:0.00}");
            }
            fillImage.fillAmount = ratio;
        }

        // 텍스트 변화 감지
        if (cooldownText != null)
        {
            if (currentCool > 0)
            {
                int time = Mathf.CeilToInt(currentCool);
                if (cooldownText.text != time.ToString())
                {
                    Debug.Log($"[{label}] 텍스트 표시 시작: {time}초 남음");
                }
                cooldownText.text = time.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(cooldownText.text))
                {
                    Debug.Log($"[{label}] 쿨타임 종료 → 텍스트 사라짐");
                }
                cooldownText.text = "";
            }
        }
    }

    // D / F 스킬 쿨타임 UI 처리 (bool 상태 기반 수동 추적)
    void UpdateDFCooldown(string label, bool isAvailable, Image fillImage, Text cooldownText, float cooldownDuration, ref float lastUsedTime)
    {
        if (isAvailable)
        {
            // 사용 가능 상태일 경우 Fill 0, 텍스트 없음
            if (fillImage != null && fillImage.fillAmount != 0f)
            {
                Debug.Log($"[{label}] 사용 가능 상태 → Fill 0, 텍스트 사라짐");
            }

            if (cooldownText != null && !string.IsNullOrEmpty(cooldownText.text))
            {
                Debug.Log($"[{label}] 사용 가능 상태 → 텍스트 초기화");
            }

            if (fillImage != null) fillImage.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";

            // 쿨타임 초기화
            lastUsedTime = Time.time;
        }
        else
        {
            float elapsed = Time.time - lastUsedTime;
            float remain = Mathf.Clamp(cooldownDuration - elapsed, 0f, cooldownDuration);
            float ratio = remain / cooldownDuration;

            if (fillImage != null && Mathf.Abs(fillImage.fillAmount - ratio) > 0.001f)
            {
                Debug.Log($"[{label}] FillAmount 감소 중: {fillImage.fillAmount:0.00} → {ratio:0.00}");
            }

            if (cooldownText != null && cooldownText.text != Mathf.CeilToInt(remain).ToString())
            {
                Debug.Log($"[{label}] 텍스트 표시 중: {Mathf.CeilToInt(remain)}초 남음");
            }

            if (fillImage != null) fillImage.fillAmount = ratio;
            if (cooldownText != null) cooldownText.text = Mathf.CeilToInt(remain).ToString();
        }
    }

    // Character.cs의 private 쿨타임 값(qCoolDown 등)을 리플렉션으로 읽어옴
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
