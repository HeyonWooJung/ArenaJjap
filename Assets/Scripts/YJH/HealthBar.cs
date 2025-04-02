using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerController target;
    public Image fillImage;
    public Vector3 offset = new Vector3(0,0,0);

    void Update()
    {
        float ratio = Mathf.Clamp01(target.character.CurHP / target.character.HP);
        fillImage.fillAmount = ratio;

        transform.position = target.transform.position + offset;
        transform.forward = Camera.main.transform.forward;
    }
}