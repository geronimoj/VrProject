Shader "Custom/WaveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Emission("Emission", Color) = (1,1,1,0)
        _WaveColour("WaveColour", Color) = (1,1,1,1)
        _WaveEmission("Wave Emission", Color) = (1, 1, 1, 0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Direction("Wave Direction (Ignore W)", Vector) = (0, 0, 0, 0)
        _Rate("Wave Rate", Float) = 1
        _Num("Wave Number", Float) = 1
    }
    SubShader
    {
        Tags {  "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 vertexPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Emission;
        fixed4 _WaveColour;
        fixed4 _WaveEmission;
        float _Rate;
        float _Num;
        float4 _Direction;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexPos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float d = dot(IN.vertexPos, _Direction.xyz);
            float percent = sin((d * _Num) + (_Time * _Rate));
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * (_Color * (1 - percent)) + (_WaveColour * percent);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = (_Emission.rgb * _Emission.a * (1 - percent)) + (_WaveEmission.rgb * _WaveEmission.a * percent);
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
