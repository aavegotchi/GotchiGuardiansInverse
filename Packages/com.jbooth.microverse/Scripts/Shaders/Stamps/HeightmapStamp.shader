Shader "Hidden/MicroVerse/HeightmapStamp"
{
    Properties
    {
        [HideInInspector] _MainTex ("Heightmap Texture", 2D) = "white" {}
        [HideInInspector] _StampTex("Stamp", 2D) = "black" {}
        [HideInInspector] _FalloffTexture("Falloff", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local_fragment _ _USEFALLOFF _USEFALLOFFRANGE _USEFALLOFFTEXTURE _USEFALLOFFSPLINEAREA
            #pragma shader_feature_local_fragment _ _TWIST
            #pragma shader_feature_local_fragment _ _EROSION
            #pragma shader_feature_local_fragment _ _HEIGHTBIAS
            #pragma shader_feature_local_fragment _ _USEORIGINALHEIGHTMAP
            #pragma shader_feature_local_fragment _ _ABSOLUTEHEIGHT
            #pragma shader_feature_local_fragment _ _PASTESTAMP

            #pragma shader_feature_local _ _FALLOFFSMOOTHSTEP _FALLOFFEASEIN _FALLOFFEASEOUT _FALLOFFEASEINOUT
            #pragma shader_feature_local _ _FALLOFFNOISE _FALLOFFFBM _FALLOFFWORLEY _FALLOFFWORM _FALLOFFWORMFBM _FALLOFFNOISETEXTURE


            #include "UnityCG.cginc"
            #include "/../Noise.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 stampUV: TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _FalloffTexture;
            float2 _HeightRemap;
            float2 _HeightRenorm;
            int _CombineMode;
            float _Twist;
            float _Erosion;
            float _ErosionSize;
            float _HeightBias;
            float4 _ScaleOffset;
            float _MipBias;
            float2 _RemapRange;
            float2 _NoiseUV;
            sampler2D _StampTex;
            float4 _StampTex_TexelSize;
            sampler2D _PlacementMask;
            float4x4 _Transform;
            float2 _Falloff;
            int _FalloffTextureChannel;
            float2 _FalloffTextureParams;
            float4 _FalloffTextureRotScale;
            float _FalloffAreaRange;
            float _AlphaMapSize;
            float3 _RealSize;

            sampler2D _FalloffNoiseTexture;
            float4 _FalloffNoiseTexture_ST;
            float4 _FalloffNoise;
            float4 _FalloffNoise2;
            int _FalloffNoiseChannel;

            float _Blend;
            float _Invert;
            float _Power;
            float3 _Tilt;
            float2 _TiltScale;
            
            
            float RectFalloff(float2 uv, float falloff) 
            {
                if (falloff == 1)
                    return 1;
                uv = saturate(uv);
                uv -= 0.5;
                uv = abs(uv);
                uv = 0.5 - uv;
                falloff = 1 - falloff;
                uv = smoothstep(uv, 0, 0.03 * falloff);
                return min(uv.x, uv.y);
            }
            

            sampler2D _OriginalHeights;

            v2f vert(vertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                o.stampUV = mul(_Transform, float4(v.uv, 0, 1)).xy;
                o.stampUV -= 0.5;
                float2 tilt = saturate(abs(_Tilt.zx));
                tilt *= tilt;
                o.stampUV *= _TiltScale > 0.5 ? lerp(1, 3.14, tilt) : 1;
                o.stampUV += 0.5;
                return o;
            }

            float CombineHeight(float oldHeight, float height, int combineMode)
            {
                switch (combineMode)
                {
                case 0:
                    return height;
                case 1:  
                    return max(oldHeight, height);
                case 2:
                    return min(oldHeight, height);
                case 3:
                    return oldHeight + height;
                case 4:
                    return oldHeight - height;
                case 5:
                    return (oldHeight * height);
                case 6:
                    return (oldHeight + height) / 2;
                case 7:
                    return abs(height-oldHeight);
                case 8:
                    return sqrt(oldHeight * height);
                case 9:
                    return lerp(oldHeight, height, _Blend);
                default:
                    return oldHeight;
                }
            }



            float3 GenerateStampNormal(float2 uv, float height, float spread)
            {
                float2 offset = _StampTex_TexelSize.xy * spread;
                float2 uvx = uv + float2(offset.x, 0.0);
                float2 uvy = uv + float2(0.0, offset.y);

                float x = tex2D(_StampTex, uvx).r;
                float y = tex2D(_StampTex, uvy).r;

                float2 dxy = height - float2(x, y);

                dxy = dxy * 1 / offset.xy;
                return normalize(float4( dxy.x, dxy.y, 1.0, height)).xzy * 0.5 + 0.5;
            }


            // radial distort UV coordinates
            float2 RadialUV(float2 uv, float2 center, float str, float2 offset)
            {
                float2 delta = uv - center;
                float delta2 = dot(delta.xy, delta.xy);
                float2 delta_offset = delta2 * str;
                return uv + float2(delta.y, -delta.x) * delta_offset + offset;
            }

            float2 RotateScaleUV(float2 uv, float2 amt)
            {
                uv -= 0.5;
                uv *= amt.y;
                if (amt.x != 0)
                {
                    float s = sin ( amt.x );
                    float c = cos ( amt.x );
                    float2x2 mtx = float2x2( c, -s, s, c);
                    mtx *= 0.5;
                    mtx += 0.5;
                    mtx = mtx * 2-1;
                    uv = mul ( uv, mtx );
                }
                uv += 0.5;
                return uv;
            }

            float ComputeFalloff(float2 uv, float2 stampUV, float2 noiseUV, float noise)
            {
                float falloff = 1;
                #if _USEFALLOFF
                    falloff = RectFalloff(stampUV, saturate(_Falloff.y - noise));
                #elif _USEFALLOFFRANGE
                {
                    float2 off = saturate(_Falloff * 0.5 - saturate(noise) * 0.5);
                    float radius = length( stampUV-0.5 );
 	                falloff = 1.0 - saturate(( radius-off.x ) / max(0.001, ( off.y-off.x )));
                }
                #elif _USEFALLOFFTEXTURE
                {
                    float falloffSample = tex2D(_FalloffTexture, RotateScaleUV(stampUV, _FalloffTextureRotScale.xy) + _FalloffTextureRotScale.zw)[_FalloffTextureChannel];
                    falloff *= falloffSample;
                    falloff *= _FalloffTextureParams.x;
                    falloff += _FalloffTextureParams.y * falloffSample;
                    falloff *= RectFalloff(stampUV, saturate(_Falloff.y - noise));
                }
                #elif _USEFALLOFFSPLINEAREA
                {
                    float d = tex2D(_FalloffTexture, uv).r;
                    d *= -1;
                    d /= max(0.0001, _FalloffAreaRange - noise);
                    falloff *= saturate(d);
                }
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
                return falloff;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 noiseUV = i.uv + _NoiseUV;
                float height = UnpackHeightmap(tex2D(_MainTex, i.uv));
                
                bool cp = (i.stampUV.x < 0 || i.stampUV.x > 1 || i.stampUV.y < 0|| i.stampUV.y > 1);
                if (cp)
                    return PackHeightmap(height);

                

                float2 stampUV = i.stampUV * _ScaleOffset.xy + _ScaleOffset.zw;

                #if _TWIST
                    stampUV = RadialUV(i.stampUV, 0.5, _Twist, 0);
                #endif

                float stamp = tex2Dbias(_StampTex, float4(stampUV, 0, _MipBias)).r;

                if (_Invert > 0.5)
                    stamp = 1.0 - stamp;
                stamp = pow(stamp, _Power);

                
                float2 tilt = lerp(float2(-1, -1), float2(1,1), stampUV) * (_Tilt.zx);
                stamp += tilt.x + tilt.y;
               

                

                #if _EROSION
                    float3 normal = GenerateStampNormal(stampUV, stamp, _ErosionSize) * 0.3333;
                    normal += GenerateStampNormal(stampUV, stamp, _ErosionSize*3) * 0.3333;
                    normal += GenerateStampNormal(stampUV, stamp, _ErosionSize*7) * 0.3334;
                    
                    float erosNoise = ErosionNoise(stampUV, normal);
                    float erosStr = (1 - normal.y);
                    erosStr *= erosStr;
                    stamp -= erosStr * erosNoise * _Erosion / _RealSize.y;
                #endif

                float2 falloffuv = noiseUV;
                if (_FalloffNoise2.x > 0)
                    falloffuv = stampUV;

                float noise = 0;
                float falloff = ComputeFalloff(i.uv, i.stampUV, noiseUV, 0);

                #if _FALLOFFNOISE
                    noise = (Noise(falloffuv, _FalloffNoise)) / _RealSize.y;
                #elif _FALLOFFFBM
                    noise = (NoiseFBM(falloffuv, _FalloffNoise)) / _RealSize.y;
                #elif _FALLOFFWORLEY
                    noise = (NoiseWorley(falloffuv, _FalloffNoise)) / _RealSize.y;
                #elif _FALLOFFWORM
                    noise = (NoiseWorm(falloffuv, _FalloffNoise)) / _RealSize.y;
                #elif _FALLOFFWORMFBM
                    noise = (NoiseWormFBM(falloffuv, _FalloffNoise)) / _RealSize.y;
                #elif _FALLOFFNOISETEXTURE
                    noise = (tex2D(_FalloffNoiseTexture, falloffuv * _FalloffNoiseTexture_ST.xy + _FalloffNoiseTexture_ST.zw)[_FalloffNoiseChannel] * 2.0 - 1.0) / _RealSize.y * _FalloffNoise.y + _FalloffNoise.w;
                #endif

                

                #if _FALLOFFNOISE || _FALLOFFFBM || _FALLOFFWORLEY || _FALLOFFWORM || _FALLOFFWORMFBM || _FALLOFFNOISETEXTURE
                    noise *= 1-falloff;
                    falloff = ComputeFalloff(i.uv, stampUV, noiseUV, noise);
                #endif

                falloff *= 1.0 - tex2D(_PlacementMask, i.uv).x;

                #if _USEORIGINALHEIGHTMAP
                    float originalHeight = UnpackHeightmap(tex2D(_OriginalHeights, i.uv));   
                    return PackHeightmap(originalHeight);
                #endif

                

                //stamp = _RemapRange.x + stamp * max(0.01, _RemapRange.y - _RemapRange.x);
                
                #if _ABSOLUTEHEIGHT
                    stamp *= _HeightRenorm.y;
                #endif

                float newHeight = saturate(_HeightRemap.x + stamp * (_HeightRemap.y - _HeightRemap.x));
          
                float blend = CombineHeight(height, newHeight, _CombineMode);
                return PackHeightmap(lerp(height, blend, falloff));
            }
            ENDCG
        }
    }
}