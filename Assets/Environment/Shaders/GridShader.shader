// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_GridColor("Grid Color", Color) = (255, 0, 0, 0)
        _Color ("Color (RGBA)", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull front 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _GridColor;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                v.uv.x = 1 - v.uv.x;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            float GridTest(float2 r) {
                float result;
                for (float i=0.0; i<2.0; i+= 0.1) {
                    for (int j=0; j<2; j++) {
                        result += 1.0 - smoothstep(0.0, 0.004, abs(r[j] - i));
                    } 
                }
                return result;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                //return col;
                fixed4 gridColor = (_GridColor * GridTest(i.uv)) + tex2D(_MainTex, i.uv) * _Color;
                return float4(gridColor);
            }
            ENDCG
        }
    }
}
