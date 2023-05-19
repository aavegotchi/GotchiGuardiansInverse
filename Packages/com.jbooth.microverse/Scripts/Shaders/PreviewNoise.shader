Shader "Hidden/MicroVerse/NoisePreview"
{
    Properties
    {
     
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Noise.cginc"

            #pragma shader_feature_local_fragment _ _NOISE _FBM _WORLEY _WORM _WORMFBM

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

            float4 _Param;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                #if _NOISE
                    return Noise(i.uv, _Param);
                #elif _FBM
                    return NoiseFBM(i.uv, _Param);
                #elif _WORM
                    return NoiseWorm(i.uv, _Param);
                #elif _WORMFBM
                    return NoiseWormFBM(i.uv, _Param);
                #else
                    return NoiseWorley(i.uv, _Param);
                #endif
            }
            ENDCG
        }
    }
}
