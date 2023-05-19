using UnityEngine;
using System.Collections.Generic;

namespace JBooth.MicroVerseCore
{
    

    [ExecuteAlways]
    public class TextureStamp : Stamp, ITextureModifier
    {
        public override FilterSet GetFilterSet()
        {
            return filterSet;
        }

        public TerrainLayer layer;

        public FilterSet filterSet = new FilterSet();
        [Tooltip("When true, we ignore occlusion stamps")]
        public bool ignoreOcclusion;

        Material material;

        RenderBuffer[] _mrt;
        static Shader splatFilterShader = null;
        public void Initialize(Terrain[] terrains)
        {
            if (splatFilterShader == null)
            {
                splatFilterShader = Shader.Find("Hidden/MicroVerse/SplatFilter");
            }
            material = new Material(splatFilterShader);
            _mrt = new RenderBuffer[2];
        }


        public Bounds GetBounds()
        {
            FalloffOverride fo = GetComponentInParent<FalloffOverride>();
            var foType = filterSet.falloffFilter.filterType;
            var foFilter = filterSet.falloffFilter;
            if (fo != null && fo.enabled)
            {
                foType = fo.filter.filterType;
                foFilter = fo.filter;
            }
#if __MICROVERSE_SPLINES__
            if (foType == FalloffFilter.FilterType.SplineArea && foFilter.splineArea != null)
            {
                return foFilter.splineArea.GetBounds();
            }
#endif

            if (foType == FalloffFilter.FilterType.Global)
                return new Bounds(Vector3.zero, new Vector3(99999, 999999, 99999));
            else
            {
                return TerrainUtil.GetBounds(transform);
            }
        }

        int channelIndex = -1;

        public void Dispose()
        {
            _mrt = null;
            DestroyImmediate(material);
        }

        public bool NeedCurvatureMap() { return filterSet.NeedCurvatureMap(); }

        static int _Heightmap = Shader.PropertyToID("_Heightmap");
        static int _Normalmap = Shader.PropertyToID("_Normalmap");
        static int _Curvemap = Shader.PropertyToID("_Curvemap");
        static int _PlacementMask = Shader.PropertyToID("_PlacementMask");
        static int _Channel = Shader.PropertyToID("_Channel");
        static int _IndexMap = Shader.PropertyToID("_IndexMap");
        static int _WeightMap = Shader.PropertyToID("_WeightMap");

        public bool ApplyTextureStamp(RenderTexture indexSrc, RenderTexture indexDest,
            RenderTexture weightSrc, RenderTexture weightDest,
            TextureData splatmapData, OcclusionData od)
        {
            if (layer == null)
                return false;

            channelIndex = TerrainUtil.FindTextureChannelIndex(od.terrain, layer);
            if (channelIndex == -1)
            {
                return false;
            }
            keywordBuilder.Clear();
            material.SetTexture(_Heightmap, splatmapData.heightMap);
            material.SetTexture(_Normalmap, splatmapData.normalMap);
            material.SetTexture(_Curvemap, splatmapData.curveMap);
            if (!ignoreOcclusion)
                material.SetTexture(_PlacementMask, od.terrainMask);

            material.SetVector("_AlphaMapSize", new Vector2(indexSrc.width, indexSrc.width));
            filterSet.PrepareMaterial(this.transform, splatmapData.terrain, material, keywordBuilder.keywords);

            material.SetFloat(_Channel, channelIndex);
            material.SetTexture(_WeightMap, weightSrc);
            material.SetTexture(_IndexMap, indexSrc);
            keywordBuilder.Assign(material);

            _mrt[0] = indexDest.colorBuffer;
            _mrt[1] = weightDest.colorBuffer;

            Graphics.SetRenderTarget(_mrt, indexDest.depthBuffer);

            Graphics.Blit(null, material, 0);
            return true;
        }


        void OnDrawGizmosSelected()
        {
            if (filterSet.falloffFilter.filterType != FalloffFilter.FilterType.Global &&
                filterSet.falloffFilter.filterType != FalloffFilter.FilterType.SplineArea)
            {
                if (MicroVerse.instance != null)
                {
                    Gizmos.color = MicroVerse.instance.options.colors.textureStampColor;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), Vector3.one);
                }
            }
        }

       

        void ITextureModifier.InqTerrainLayers(Terrain terrain, List<TerrainLayer> layers)
        {
            layers.Add(layer);
        }

    }
}