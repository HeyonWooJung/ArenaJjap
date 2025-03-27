using UnityEngine;

public class FogSystem : MonoBehaviour
{
    public Material fogMaterial;
    public RenderTexture visionMask;
    public Texture preRenderedMapMask; // 미리 찍어놓은 맵 마스크 (흰색 전체 맵 이미지, 적 제외)

    void Start()
    {
        // visionMask는 적을 제외한 시야만 보여줄 용도
        Shader.SetGlobalTexture("_VisionTex", visionMask);
        Shader.SetGlobalTexture("_MapTex", preRenderedMapMask); // 전체 맵 이미지

        fogMaterial.SetTexture("_VisionTex", visionMask);
        fogMaterial.SetTexture("_MapTex", preRenderedMapMask);
    }
}