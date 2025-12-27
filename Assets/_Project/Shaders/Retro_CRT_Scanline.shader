Shader "UI/Retro_CRT_Scanline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(CRT Settings)]
        _ScanlineSize ("Scanline Size", Float) = 100.0
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.1
        _Distortion ("Distortion", Range(-1, 1)) = 0.1
        _Aberration ("Chromatic Aberration", Range(0, 0.05)) = 0.005
        
        // UI Masking support
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _WriteMask ("Stencil Write Mask", Float) = 255
        _ReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_ReadMask]
            WriteMask [_WriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;
            float4 _MainTex_ST;
            
            float _ScanlineSize;
            float _ScanlineIntensity;
            float _NoiseStrength;
            float _Distortion;
            float _Aberration;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            // CRT Curve Distortion
            float2 warp(float2 uv)
            {
                float2 dc = abs(0.5 - uv);
                dc *= dc;
                uv.x -= 0.5; uv.x *= 1.0 + (dc.y * (0.3 * _Distortion)); uv.x += 0.5;
                uv.y -= 0.5; uv.y *= 1.0 + (dc.x * (0.4 * _Distortion)); uv.y += 0.5;
                return uv;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = warp(IN.texcoord);
                
                // Vignette (Dark corners)
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return fixed4(0,0,0,1);

                // Chromatic Aberration (RGB Split)
                float red = tex2D(_MainTex, uv + float2(_Aberration, 0)).r;
                float green = tex2D(_MainTex, uv).g;
                float blue = tex2D(_MainTex, uv - float2(_Aberration, 0)).b;
                fixed4 color = fixed4(red, green, blue, 1.0);
                
                // Scanlines
                float scanline = sin(uv.y * _ScanlineSize * 3.14159 * 2.0);
                color.rgb -= scanline * _ScanlineIntensity;

                // Noise
                float noise = frac(sin(dot(uv * _Time.y, float2(12.9898, 78.233))) * 43758.5453);
                color.rgb += (noise - 0.5) * _NoiseStrength;

                // Apply UI Color & Alpha
                color *= IN.color;
                
                // Clipping for ScrollViews
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                // Masking support
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}