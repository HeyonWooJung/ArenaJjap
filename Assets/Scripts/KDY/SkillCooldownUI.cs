using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("쿨타임 설정")]
    public KeyCode key;                // Q, W, E, R, D, F 키
    public float cooldownTime = 5f;    // 쿨타임 (초)

    [Header("UI 연결")]
    public Image fillImage;           // Filled 이미지 (Radial)
    public Text cooldownText;         // 쿨타임 텍스트 (Legacy UI Text)

    private bool isCoolingDown = false;

    void Start()
    {
        fillImage.fillAmount = 0f;
        fillImage.enabled = false;         //렌더링만 끄기 (SetActive X)
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
        fillImage.enabled = true;          // 다시 보이게
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
        fillImage.enabled = false;        // 다시 숨기기
        cooldownText.text = "";
        isCoolingDown = false;
    }
}
