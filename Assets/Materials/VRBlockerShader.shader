Shader "Custom/VRBlocker_Unlit_CullOff"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0.6)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Cull Off           // ✅ 핵심! 내부 면도 렌더링
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Color [_Color]
        }
    }
}
