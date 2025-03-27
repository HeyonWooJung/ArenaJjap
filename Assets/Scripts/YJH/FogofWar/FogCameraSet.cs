using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FogCameraSet : MonoBehaviour
{
    public RenderTexture visionMask;

    void Start()
    {
        var cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.gray;
        cam.cullingMask = LayerMask.GetMask("FogReveal");
        cam.targetTexture = visionMask;
    }
}