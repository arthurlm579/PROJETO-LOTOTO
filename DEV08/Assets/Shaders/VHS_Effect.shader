Shader "Hidden/VHS_Effect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}   // opcional, mas melhora
        _Intensity ("Intensity", Range(0, 1)) = 0.65
        _ScanlineSpeed ("Scanline Speed", Float) = 8.0
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.4
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.05)) = 0.012
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _Intensity;
            float _ScanlineSpeed;
            float _GlitchAmount;
            float _ChromaticAberration;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;

                // Scanlines horizontais
                float scanline = sin(uv.y * _ScreenParams.y * 1.8 + time * _ScanlineSpeed) * 0.035;

                // Glitch horizontal (deslocamento aleatório)
                float glitch = (frac(sin(time * 12.0) * 43758.5453) * _GlitchAmount - _GlitchAmount * 0.5) * step(0.96, frac(time * 4.0));

                uv.x += glitch * 0.08;

                // Chromatic Aberration (RGB split)
                fixed4 colR = tex2D(_MainTex, uv + float2(_ChromaticAberration, 0));
                fixed4 colG = tex2D(_MainTex, uv);
                fixed4 colB = tex2D(_MainTex, uv - float2(_ChromaticAberration, 0));

                fixed4 col = fixed4(colR.r, colG.g, colB.b, 1.0);

                // Noise / Grain VHS
                float noise = tex2D(_NoiseTex, uv * 12.0 + time * 12.0).r * 0.08;

                // Aplicar scanline + noise
                col.rgb += scanline;
                col.rgb += noise;

                // Escurece um pouco nas bordas (vignette sutil)
                float vignette = 1.0 - dot(uv - 0.5, uv - 0.5) * 0.8;
                col.rgb *= vignette * 0.98 + 0.02;

                // Intensidade geral
                col.rgb = lerp(col.rgb, fixed3(0.95, 0.98, 1.0), _Intensity * 0.15); // leve tom azulado esverdeado

                return col;
            }
            ENDCG
        }
    }
}