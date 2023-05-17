using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    public class HoleStamp : Stamp, IHoleModifier
    {
        public override FilterSet GetFilterSet()
        {
            return filterSet;
        }


        public void Initialize(Terrain[] terrains)
        {

        }

        public bool IsValidHoleStamp() { return true; }

        public FilterSet filterSet = new FilterSet();

        static Shader holeShader = null;
        static Material material;
        public void ApplyHoleStamp(RenderTexture src, RenderTexture dest, HoleData md, OcclusionData od)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Hole Modifier");

            if (holeShader == null)
            {
                holeShader = Shader.Find("Hidden/MicroVerse/HoleStamp");
            }
            var textureLayerWeights = filterSet.GetTextureWeights(md.terrain.terrainData.terrainLayers);
            if (material == null)
                material = new Material(holeShader);
            keywordBuilder.Clear();
            keywordBuilder.Add("_RECONSTRUCTNORMAL");
            material.SetTexture("_Heightmap", md.heightMap);
            material.SetTexture("_Normalmap", md.normalMap);
            material.SetTexture("_Curvemap", md.curveMap);
            material.SetTexture("_IndexMap", md.indexMap);
            material.SetTexture("_WeightMap", md.weightMap);
            material.SetVectorArray("_TextureLayerWeights", textureLayerWeights);
            filterSet.PrepareMaterial(this.transform, md.terrain, material, keywordBuilder.keywords);

            keywordBuilder.Assign(material);
            Graphics.Blit(src, dest, material);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void Dispose()
        {
            DestroyImmediate(material);
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

        void OnDrawGizmosSelected()
        {
            if (MicroVerse.instance != null)
            {
                Gizmos.color = Color.grey;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), Vector3.one);
            }
        }

        public bool NeedCurvatureMap()
        {
            return (filterSet.curvatureFilter.enabled);
        }

        
    }
}
