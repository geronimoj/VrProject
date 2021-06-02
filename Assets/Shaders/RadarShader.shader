Shader "Custom/RadarShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Emission("Emission", Color) = (1, 1, 1, 0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _VertLines("Number of Vertical Lines", Int) = 1
        _HozLines("Number of Horizontal Lines", Int) = 1
        _Width("Hoz Width", Range(0, 0.5)) = 0.1
        _WidthVert("Vert Width", Int) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 200
        Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert alpha:fade

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
        float _VertLines;
        float _HozLines;
        float _Width;
        float _WidthVert;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexPos = v.vertex;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            int kill = 0;
            //Get the spacing between each horizontal line
            float spacing = 1 / (_HozLines + 1);
            //Put the y in 0 - 1 range
            float yPos = IN.vertexPos.y + 0.5f;
            //If the y position is below the bottom ring or above the top ring, clip them
            kill -= step(yPos, _Width / 2);
            kill -= step(1 - _Width / 2, yPos);
            //Use the modulus operator to get the distance from the lowest line
            yPos %= spacing;
            //if yPos is between 0 & width / 2
            kill += step(yPos, _Width / 2);
            //if yPos is between spacing & spacing - width / 2
            kill += step(spacing - _Width / 2, yPos);

            float2 vec = float2(IN.vertexPos.x, IN.vertexPos.z);
            vec = normalize(vec);
            //We do a similar thing for the vertical lines but instead using angles
            //Increment the angle by 180 to put it in 0 - 360 range
            float angle = degrees(atan2(vec.y,vec.x)) + 180;
            spacing = 360 / _VertLines;
            //Put the angle into steps
            angle %= spacing;
            //If angle is between 0 & width / 2 don't kill it
            kill += step(angle, _WidthVert / 2);
            //If the angle is between spacing & spacing - width / 2
            kill += step(spacing - _WidthVert / 2, angle);

            clip(kill - 1);
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = _Emission.rgb * _Emission.a;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
