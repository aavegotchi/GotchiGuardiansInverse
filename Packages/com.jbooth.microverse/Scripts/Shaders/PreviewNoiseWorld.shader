Shader "Hidden/MicroVerse/PreviewNoiseWorld"
{
    Properties
    {

    }
    SubShader
    {
        Cull Back ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "TerrainPreview.cginc"

        
        
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #pragma shader_feature_local_fragment _ _NOISE _FBM _WORLEY _WORM _WORMFBM _NOISETEXTURE
            #include "Noise.cginc"

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 pcPixels : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
            };

 
            float4 _Param;
            float4 _Param2;
            float2 _NoiseUV;
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            float _NoiseChannel;
            float2 _TerrainSize;
            float4 _Color;

            Varyings vert(uint vid : SV_VertexID)
            {
                Varyings o;
                
                // build a quad mesh, with one vertex per paint context pixel (pcPixel)

                float2 pcPixels = BuildProceduralQuadMeshVertex(vid);

                // compute heightmap UV and sample heightmap
                float2 heightmapUV = PaintContextPixelsToHeightmapUV(pcPixels);
                float heightmapSample = UnpackHeightmap(tex2Dlod(_Heightmap, float4(heightmapUV, 0, 0)));

                // compute brush UV
                float2 brushUV = PaintContextPixelsToBrushUV(pcPixels);

                // compute object position (in terrain space) and world position

                float3 positionObject = PaintContextPixelsToObjectPosition(pcPixels, heightmapSample);
                float3 positionWorld = TerrainObjectToWorldPosition(positionObject);

                o.pcPixels = pcPixels;
                o.positionCS = UnityWorldToClipPos(positionWorld);
                o.worldPos = positionWorld;
                o.uv = brushUV;
                return o;
            }


            float4 frag(Varyings i) : SV_Target
            {
                float2 noiseUV = (i.worldPos.xz/_TerrainSize);
                if (_Param2.x > 0)
                {
                    // TODO: Add all the data to do stuff in stamp space to make this work..
                    return 0; // noiseUV = i.uv;
                }
                float4 color = _Color;
                #if _NOISE
                    return saturate(Noise(noiseUV, _Param)) * color;
                #elif _FBM
                    return saturate(NoiseFBM(noiseUV, _Param)) * color;
                #elif _WORM
                    return saturate(NoiseWorm(noiseUV, _Param)) * color;
                #elif _WORMFBM
                    return saturate(NoiseWormFBM(noiseUV, _Param)) * color;
                #elif _WORLEY
                    return saturate(NoiseWorley(noiseUV, _Param)) * color;
                #else 
                    return ((tex2D(_NoiseTexture, noiseUV * _NoiseTexture_ST.xy + _NoiseTexture_ST.zw)[_NoiseChannel]) * _Param.y + _Param.w) * color;
                #endif
            }
            ENDHLSL
        }
    }
    Fallback Off
}