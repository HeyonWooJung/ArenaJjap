Shader "Custom/FogAddBlend"
{
    SubShader
    {
        Pass
        {
            Blend One One
            SetTexture [_MainTex] { combine texture }
        }
    }
}