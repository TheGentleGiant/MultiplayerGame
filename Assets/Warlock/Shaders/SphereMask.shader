Shader "Custom/SphereMask"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _NormalMap("Normals", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float3 _GMaskPosition;
        float _GMaskRadius;
        float _GMaskFeather;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input i, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, i.uv_MainTex) * _Color;

            half dst = distance(_GMaskPosition, i.worldPos);
            half mask = saturate((dst - _GMaskRadius) / _GMaskFeather);

            clip(mask - 0.1);

            o.Albedo = c.rgb;
            o.Normal = normalize(UnpackScaleNormal(tex2D(_NormalMap, i.uv_MainTex), 0.25).xzy);
            o.Alpha = c.a;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
