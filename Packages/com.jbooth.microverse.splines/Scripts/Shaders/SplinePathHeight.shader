Shader "Hidden/MicroVerse/SplinePathHeight"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Heightmap Texture", 2D) = "white" {}
        _SplineSDF("Spline SDF", 2D) = "black" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local _ _FALLOFFSMOOTHSTEP _FALLOFFEASEIN _FALLOFFEASEOUT _FALLOFFEASEINOUT
            #pragma shader_feature_local _ _FALLOFFNOISE _FALLOFFFBM _FALLOFFWORLEY _FALLOFFWORM _FALLOFFWORMFBM _FALLOFFNOISETEXTURE
            #pragma shader_feature_local _ _HEIGHTNOISE _HEIGHTFBM _HEIGHTWORLEY _HEIGHTWORM _HEIGHTWORMFBM _HEIGHTNOISETEXTURE

            #pragma shader_feature_local _ _TREATASAREA


            #include "UnityCG.cginc"
            #include "Packages/com.jbooth.microverse/Scripts/Shaders/Noise.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };



            sampler2D _MainTex;
            sampler2D _SplineSDF;
            float4 _SplineSDF_TexelSize;

            float _Width;
            float _Smoothness;
            float _Trench;
            float _RealHeight;
            float2 _NoiseUV;
            float _TerrainHeight;
            sampler2D _FalloffNoiseTexture;
            float4 _FalloffNoiseTexture_ST;
            float4 _FalloffNoise;
            int _FalloffNoiseChannel;

            sampler2D _HeightNoiseTexture;
            float4 _HeightNoiseTexture_ST;
            float4 _HeightNoise;
            int _HeightNoiseChannel;

            v2f vert(vertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp(float a, float b, float t)
            {
                return (t - a) / max(0.001, (b - a));
            }

            float Blend(float width, float smoothness, float d)
            {
                #if _BLENDSMOOTHSTEP
                    return 1.0 - smoothstep(width, width + smoothness, d);
                #else
                    float v = saturate(InverseLerp(width, width + smoothness, d));

                    #if _BLENDEASEOUT
                        v = 1 - (1-v) * (1-v);
                    #elif _BLENDEASEIN
                        v = v * v;
                    #elif _BLENDEASEINOUT
                        v = v < 0.5 ? 2 * v * v : 1 - pow(-2 * v + 2, 2) * 0.5;
                    #endif

                    
                    return 1 - v;
                #endif
                
            }

            float4 frag(v2f i) : SV_Target
            {
                float height = UnpackHeightmap(tex2D(_MainTex, i.uv));
                float4 os = _SplineSDF_TexelSize;
                float2 noiseUV = i.uv + _NoiseUV;
                float2 sdfUV = i.uv - os.xy * 0.5;
                float3 data = tex2D(_SplineSDF, sdfUV).xyz;
                
                
                float falloff = Blend(_Width, _Smoothness, data.g);
                #if _TREATASAREA
                if (data.r < 0) falloff = 1;
                #endif

                #if _HEIGHTNOISE
                    data.b += ((Noise(noiseUV, _HeightNoise) * falloff));
                #elif _HEIGHTFBM
                    data.b += ((NoiseFBM(noiseUV, _HeightNoise) * falloff));
                #elif _HEIGHTWORLEY
                    data.b += ((NoiseWorley(noiseUV, _HeightNoise) * falloff));
                #elif _HEIGHTWORM
                    data.b += ((NoiseWorm(noiseUV, _HeightNoise) * falloff));
                #elif _HEIGHTWORMFBM
                    data.b += ((NoiseWormFBM(noiseUV, _HeightNoise) * falloff));
                #elif _HEIGHTNOISETEXTURE
                    float hnoise = abs(tex2D(_HeightNoiseTexture, noiseUV * _HeightNoiseTexture_ST.xy + _HeightNoiseTexture_ST.zw)[_HeightNoiseChannel] * 2.0 - 1.0) * _HeightNoise.y + _HeightNoise.w;
                    data.b += (hnoise * falloff);
                #endif


                
                #if _FALLOFFSMOOTHSTEP
                    falloff = smoothstep(0,1,falloff);
                #elif _FALLOFFEASEIN
                    falloff *= falloff;
                #elif _FALLOFFEASEOUT
                    falloff = 1 - (1 - falloff) * (1 - falloff);
                #elif _FALLOFFEASEINOUT
                    falloff = falloff < 0.5 ? 2 * falloff * falloff : 1 - pow(-2 * falloff + 2, 2) / 2;
                #endif

                #if _FALLOFFNOISE || _FALLOFFFBM || _FALLOFFWORLEY || _FALLOFFWORM || _FALLOFFWORMFBM || _FALLOFFNOISETEXTURE
                   float nstr = 1.0 - abs(falloff - 0.5) * 2;
                   nstr = 1 - (1-nstr) * (1-nstr);
                #endif

                #if _FALLOFFNOISE
                    data.b -= abs((Noise(noiseUV, _FalloffNoise) * nstr));
                #elif _FALLOFFFBM
                    data.b -= abs((NoiseFBM(noiseUV, _FalloffNoise) * nstr));
                #elif _FALLOFFWORLEY
                    data.b -= abs((NoiseWorley(noiseUV, _FalloffNoise) * nstr));
                #elif _FALLOFFWORM
                    data.b -= abs((NoiseWorm(noiseUV, _FalloffNoise) * nstr));
                #elif _FALLOFFWORMFBM
                    data.b -= ((NoiseWormFBM(noiseUV, _FalloffNoise) * nstr));
                #elif _FALLOFFNOISETEXTURE
                    float fnoise = abs(tex2D(_FalloffNoiseTexture, noiseUV * _FalloffNoiseTexture_ST.xy + _FalloffNoiseTexture_ST.zw)[_FalloffNoiseChannel] * 2.0 - 1.0) / _RealHeight * _FalloffNoise.y + _FalloffNoise.w;
                    data.b -= (fnoise * nstr);
                #endif
                data.b -= _TerrainHeight;
                data.b = lerp(data.b, data.b-(_Trench), 1.0 - saturate(smoothstep(_Width, _Width+_Smoothness, data.g)));
                float newHeight = saturate(lerp(height, data.b/_RealHeight, falloff));

                return PackHeightmap(newHeight);
            }
            ENDCG
        }
    }
}