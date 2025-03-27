using UnityEngine;

public class FogSystem : MonoBehaviour
{
    public Material fogMaterial;
    public RenderTexture visionMask;
    public RenderTexture seenMask;
    public Material blendMaterial; // Simple add shader

    void Start()
    {
        Shader.SetGlobalTexture("_VisionTex", visionMask);
        Shader.SetGlobalTexture("_SeenTex", seenMask);

        fogMaterial.SetTexture("_VisionTex", visionMask);
        fogMaterial.SetTexture("_SeenTex", seenMask);
    }

    void LateUpdate()
    {
        Graphics.Blit(visionMask, seenMask, blendMaterial);
    }
}
