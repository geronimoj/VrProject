Shader "Custom/RenderBelowValue"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _CubeTop ("Cube Top", Cube) = "" {}
        _CubeBot("Cube Bot", Cube) = "" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Offset("Render Below Value", Range(-1, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        ZTest Less
        Cull Off
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        samplerCUBE _CubeTop;
        samplerCUBE _CubeBot;

        struct Input
        {
            float2 uv_MainTex;
            float3 vertexPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Offset;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexPos = v.vertex.xyz;
        }
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 p = mul(unity_ObjectToWorld, float4(IN.vertexPos.xyz, 1));

            int renderTop = IN.vertexPos.y < _Offset ? 1 : 0;
            // Albedo comes from a texture tinted by color
            fixed4 c = (texCUBE (_CubeBot, IN.vertexPos.xyz) * renderTop + texCUBE(_CubeTop, IN.vertexPos.xyz) * (1 - renderTop)) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
      FallBack "Diffuse"
}