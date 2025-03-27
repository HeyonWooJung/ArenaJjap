using UnityEngine;

public class FogUnit : MonoBehaviour
{
    public RenderTexture visionMask; // FogCamera가 출력하는 텍스처
    public Camera fogCamera;         // FogCamera 객체

    Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // 처음엔 무조건 보이게 시작 (네트워크 유닛이 안 보이는 문제 방지)
        foreach (var r in renderers)
            r.enabled = true;
    }

    void LateUpdate()
    {
        if (fogCamera == null || visionMask == null)
            return;

        Vector3 viewportPos = fogCamera.WorldToViewportPoint(transform.position);
        bool inView = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;

        float visible = 0f;

        if (inView)
        {
            RenderTexture.active = visionMask;
            Texture2D tempTex = new Texture2D(1, 1, TextureFormat.RFloat, false);
            tempTex.ReadPixels(new Rect(viewportPos.x * visionMask.width, viewportPos.y * visionMask.height, 1, 1), 0, 0);
            tempTex.Apply();
            visible = tempTex.GetPixel(0, 0).r;
            RenderTexture.active = null;
            Destroy(tempTex);
        }

        bool isVisibleToLocal = visible > 0.1f;

        foreach (var r in renderers)
            r.enabled = isVisibleToLocal;
    }
}