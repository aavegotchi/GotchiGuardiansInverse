using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    public class OcclusionData : StampData
    {
        public RenderTexture terrainMask; // collective mask for all of terrain
        public RenderTexture treeSDF;     // collective sdf for all trees
        public RenderTexture currentTreeMask; // buffer for current tree stamps mask
        public RenderTexture currentTreeSDF;
        public RenderTexture objectSDF;
        public RenderTexture currentObjectMask;
        public RenderTexture currentObjectSDF;

        public RenderTexture objectMask;

        public OcclusionData(Terrain terrain, int maskSize) : base(terrain)
        {
            this.terrain = terrain;
            RenderTextureDescriptor desc = new RenderTextureDescriptor(maskSize, maskSize, RenderTextureFormat.ARGB32, 0, 0);
            desc.enableRandomWrite = true;
            desc.autoGenerateMips = false;
            terrainMask = RenderTexture.GetTemporary(desc);
            terrainMask.name = "OcclusionData::mask";
            terrainMask.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = terrainMask;
            GL.Clear(false, true, Color.clear);
            RenderTexture.active = null;

            desc = new RenderTextureDescriptor(maskSize, maskSize, RenderTextureFormat.R8, 0, 0);
            objectMask = RenderTexture.GetTemporary(desc);
            objectMask.name = "OcclusionData::mask";
            objectMask.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = objectMask;
            GL.Clear(false, true, Color.clear);
            RenderTexture.active = null;

        }

        static Shader nineCombineShader = null;
        static Shader combineSDFShader = null;
        public void RenderTreeSDF(Terrain t, Dictionary<Terrain, OcclusionData> ods, bool others)
        {
            // we have to blend across edges, so we make the render texture target larger
            // than the mask data, blit all nine into a single texture, sdf it, then
            // get the middle bit back out, then merge with cumulative texture
            if (!ods.ContainsKey(t))
            {
                return;
            }
            var myMask = ods[t].currentTreeMask;
            if (myMask == null)
                return;
            UnityEngine.Profiling.Profiler.BeginSample("Render Tree SDF");
            var expandedRT = RenderTexture.GetTemporary((int)(myMask.width * 1.25f), (int)(myMask.height * 1.25f), 0, RenderTextureFormat.R8);
            expandedRT.name = "MicroVerse::OcclusionExpandedRT";
            if (nineCombineShader == null)
            {
                nineCombineShader = Shader.Find("Hidden/MicroVerse/NineCombine");
            }
            Material mat = new Material(nineCombineShader);
            mat.SetTexture("_Tex4", myMask);
            mat.SetFloat("_Zoom", 1.25f);
            if (t.topNeighbor != null)
            {
                mat.SetTexture("_Tex1", ods[t.topNeighbor].currentTreeMask);
                if (t.topNeighbor.leftNeighbor != null)
                {
                    mat.SetTexture("_Tex0", ods[t.topNeighbor.leftNeighbor].currentTreeMask);
                }
                if (t.topNeighbor.rightNeighbor != null)
                {
                    mat.SetTexture("_Tex2", ods[t.topNeighbor.rightNeighbor].currentTreeMask);
                }
            }
            if (t.leftNeighbor)
            {
                mat.SetTexture("_Tex3", ods[t.leftNeighbor].currentTreeMask);
            }
            if (t.rightNeighbor)
            {
                mat.SetTexture("_Tex5", ods[t.rightNeighbor].currentTreeMask);
            }
            if (t.bottomNeighbor != null)
            {
                mat.SetTexture("_Tex7", ods[t.bottomNeighbor].currentTreeMask);
                if (t.bottomNeighbor.leftNeighbor != null)
                {
                    mat.SetTexture("_Tex6", ods[t.bottomNeighbor.leftNeighbor].currentTreeMask);
                }
                if (t.bottomNeighbor.rightNeighbor != null)
                {
                    mat.SetTexture("_Tex8", ods[t.bottomNeighbor.rightNeighbor].currentTreeMask);
                }
            }
            Graphics.Blit(null, expandedRT, mat);
            Object.DestroyImmediate(mat);
            if (currentTreeSDF != null)
            {
                RenderTexture.ReleaseTemporary(currentTreeSDF);
            }
            currentTreeSDF = JumpFloodSDF.CreateTemporaryRT(expandedRT, 0, 1.25f, 2);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(expandedRT);
            if (others)
            {
                if (combineSDFShader == null)
                {
                    combineSDFShader = Shader.Find("Hidden/MicroVerse/CombineSDF");
                }
                mat = new Material(combineSDFShader);
                RenderTexture rt = RenderTexture.GetTemporary(currentTreeSDF.descriptor);
                rt.name = "MicroVerse::CombinedTreeSDF";
                mat.SetTexture("_SourceA", currentTreeSDF);
                mat.SetTexture("_SourceB", treeSDF);
                Graphics.Blit(null, rt, mat);
                if (treeSDF != null)
                {
                    RenderTexture.active = null;
                    RenderTexture.ReleaseTemporary(treeSDF);
                }
                treeSDF = rt;
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void RenderObjectSDF(Terrain t, Dictionary<Terrain, OcclusionData> ods, bool others)
        {
            // we have to blend across edges, so we make the render texture target larger
            // than the mask data, blit all nine into a single texture, sdf it, then
            // get the middle bit back out, then merge with cumulative texture
            if (!ods.ContainsKey(t))
            {
                return;
            }
            var myMask = ods[t].currentObjectMask;
            if (myMask == null)
                return;
            var expandedRT = RenderTexture.GetTemporary((int)(myMask.width * 1.25f), (int)(myMask.height * 1.25f), 0, RenderTextureFormat.RG16);
            expandedRT.name = "MicroVerse::OcclusionExpandedRT";
            if (nineCombineShader == null)
            {
                nineCombineShader = Shader.Find("Hidden/MicroVerse/NineCombine");
            }
            Material mat = new Material(nineCombineShader);
            mat.SetTexture("_Tex4", myMask);
            mat.SetFloat("_Zoom", 1.25f);
            if (t.topNeighbor != null)
            {
                mat.SetTexture("_Tex1", ods[t.topNeighbor].currentObjectMask);
                if (t.topNeighbor.leftNeighbor != null)
                {
                    mat.SetTexture("_Tex0", ods[t.topNeighbor.leftNeighbor].currentObjectMask);
                }
                if (t.topNeighbor.rightNeighbor != null)
                {
                    mat.SetTexture("_Tex2", ods[t.topNeighbor.rightNeighbor].currentObjectMask);
                }
            }
            if (t.leftNeighbor)
            {
                mat.SetTexture("_Tex3", ods[t.leftNeighbor].currentObjectMask);
            }
            if (t.rightNeighbor)
            {
                mat.SetTexture("_Tex5", ods[t.rightNeighbor].currentObjectMask);
            }
            if (t.bottomNeighbor != null)
            {
                mat.SetTexture("_Tex7", ods[t.bottomNeighbor].currentObjectMask);
                if (t.bottomNeighbor.leftNeighbor != null)
                {
                    mat.SetTexture("_Tex6", ods[t.bottomNeighbor.leftNeighbor].currentObjectMask);
                }
                if (t.bottomNeighbor.rightNeighbor != null)
                {
                    mat.SetTexture("_Tex8", ods[t.bottomNeighbor.rightNeighbor].currentObjectMask);
                }
            }
            Graphics.Blit(null, expandedRT, mat);
            Object.DestroyImmediate(mat);
            currentObjectSDF = JumpFloodSDF.CreateTemporaryRT(expandedRT, 0, 1.25f, 2);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(expandedRT);

            if (others)
            {
                if (combineSDFShader == null)
                {
                    combineSDFShader = Shader.Find("Hidden/MicroVerse/CombineSDF");
                }
                mat = new Material(combineSDFShader);
                RenderTexture rt = RenderTexture.GetTemporary(currentObjectSDF.descriptor);
                rt.name = "MicroVerse::CombinedObjectSDF";
                mat.SetTexture("_SourceA", currentObjectSDF);
                mat.SetTexture("_SourceB", objectSDF);
                Graphics.Blit(null, rt, mat);
                if (objectSDF != null)
                {
                    RenderTexture.active = null;
                    RenderTexture.ReleaseTemporary(objectSDF);
                }
                objectSDF = rt;
            }
        }

        public void Dispose()
        {
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(terrainMask);
            terrainMask = null;
            RenderTexture.ReleaseTemporary(objectMask);
            objectMask = null;

            if (treeSDF != null)
            {
                RenderTexture.ReleaseTemporary(treeSDF);
            }
            if (currentTreeMask != null)
            {
                RenderTexture.ReleaseTemporary(currentTreeMask);
            }
            if (currentTreeSDF != null)
            {
                RenderTexture.ReleaseTemporary(currentTreeSDF);
            }
            if (objectSDF != null)
            {
                RenderTexture.ReleaseTemporary(objectSDF);
            }
            if (currentObjectMask != null)
            {
                RenderTexture.ReleaseTemporary(currentObjectMask);
            }
            if (currentObjectSDF != null)
            {
                RenderTexture.ReleaseTemporary(currentObjectSDF);
            }

            currentTreeMask = null;
            treeSDF = null;
            currentTreeSDF = null;
            objectSDF = null;
            currentObjectMask = null;
            currentObjectSDF = null;
        }
    }

    [ExecuteInEditMode]
    public class OcclusionStamp : Stamp, IHeightModifier, ITextureModifier
#if __MICROVERSE_VEGETATION__
        , ITreeModifier, IDetailModifier
#endif
#if __MICROVERSE_OBJECTS__
        , IObjectModifier
#endif
    {
        [Tooltip("How much to prevent future height stamps in the heriarchy from affecting this area")]
        [Range(0, 1)] public float occludeHeightWeight;
        [Tooltip("How much to prevent future texture stamps in the heriarchy from affecting this area")]
        [Range(0, 1)] public float occludeTextureWeight;
        [Tooltip("How much to prevent future tree stamps in the heriarchy from affecting this area")]
        [Range(0, 1)] public float occludeTreeWeight;
        [Tooltip("How much to prevent future detail stamps in the heriarchy from affecting this area")]
        [Range(0, 1)] public float occludeDetailWeight;
        [Tooltip("How much to prevent future objects from affecting this area")]
        [Range(0, 1)] public float occludeObjectWeight;

        public FilterSet filterSet = new FilterSet();

        Material material;
        static Shader occlusionShader = null;
        public void Initialize(Terrain[] terrains)
        {
            if (occlusionShader == null)
            {
                occlusionShader = Shader.Find("Hidden/MicroVerse/OccludeLayer");
            }
            material = new Material(occlusionShader);
        }

#if __MICROVERSE_VEGETATION__
        public bool NeedTreeClear() { return false; }
        public void ApplyTreeClear(TreeData td) { }
        public bool NeedDetailClear() { return false; }
        public void ApplyDetailClear(DetailData td) { }
        public bool UsesOtherTreeSDF() { return false; }
        public bool UsesOtherObjectSDF() { return false; }
#endif

        public override FilterSet GetFilterSet()
        {
            return filterSet;
        }

        void PrepareMaterial(Material material, OcclusionData od, List<string> keywords)
        {
            
            material.SetMatrix("_Transform", TerrainUtil.ComputeStampMatrix(od.terrain, transform)); ;
            material.SetVector("_RealSize", TerrainUtil.ComputeTerrainSize(od.terrain));

            keywordBuilder.Add("_RECONSTRUCTNORMAL");
            filterSet.PrepareMaterial(this.transform, od.terrain, material, keywords);
        }

        void Render(OcclusionData od)
        {
            RenderTexture temp = RenderTexture.GetTemporary(od.terrainMask.descriptor);
            temp.name = "Occlusion::Render::Temp";
            material.SetTexture("_MainTex", od.terrainMask);
            Graphics.Blit(od.terrainMask, temp, material);
            RenderTexture.ReleaseTemporary(od.terrainMask);
            od.terrainMask = temp;
        }
        public bool ApplyHeightStamp(RenderTexture source, RenderTexture dest, HeightmapData heightmapData, OcclusionData od)
        {
            // we don't modify the heightmaps, rather the occlusion maps, so always return
            // false so buffers aren't swapped.

            if (occludeHeightWeight <= 0)
            {
                return false;
            }
            keywordBuilder.Clear();
            PrepareMaterial(material, od, keywordBuilder.keywords);
            keywordBuilder.Assign(material);
            material.SetVector("_Mask", new Vector4(occludeHeightWeight, 0, 0, 0));
            Render(od);
            return false;
        }

        public bool ApplyTextureStamp(RenderTexture indexSrc, RenderTexture indexDest, RenderTexture weightSrc, RenderTexture weightDest,
            TextureData splatmapData, OcclusionData od)
        {
            if (occludeTextureWeight <= 0)
            {
                return false;
            }
            keywordBuilder.Clear();
            keywordBuilder.Add("_ISSPLAT");
            PrepareMaterial(material, od, keywordBuilder.keywords);
            keywordBuilder.Assign(material);
            // we don't render into the occlusion mask, because splat layers
            // render in reverse order, so instead of lower the weights of the
            // layers before us. 
            material.SetVector("_Mask", new Vector4(0, occludeTextureWeight, 0, 0));
            material.SetTexture("_MainTex", weightSrc);
            Graphics.Blit(weightSrc, weightDest, material);
            Graphics.Blit(indexSrc, indexDest);
            return true;

        }


#if __MICROVERSE_VEGETATION__
        public void ApplyTreeStamp(TreeData vd, Dictionary<Terrain, List<TreeJobHolder>> jobs,
            OcclusionData od)
        {
            if (occludeTreeWeight <= 0)
            {
                return;
            }

            keywordBuilder.Clear();
            PrepareMaterial(material, od, keywordBuilder.keywords);
            keywordBuilder.Assign(material);

            material.SetVector("_Mask", new Vector4(0, 0, occludeTreeWeight, 0));
            Render(od);
        }

        public void ProcessTreeStamp(TreeData vd, Dictionary<Terrain, List<TreeJobHolder>> jobs, OcclusionData od)
        {
            
        }

        public bool OccludesOthers()
        {
            return true;
        }

        public bool NeedSDF() { return false; }
        public bool NeedParentSDF() { return false; }
        public bool NeedToGenerateSDFForChilden() { return false;  }
        public void SetSDF(Terrain t, RenderTexture rt) { }
        public RenderTexture GetSDF(Terrain t) { return null; }




        public void ApplyDetailStamp(DetailData dd, Dictionary<Terrain, Dictionary<int, List<RenderTexture>>> resultBuffers,
            OcclusionData od)
        {
            if (occludeDetailWeight <= 0)
            {
                return;
            }
            keywordBuilder.Clear();
            PrepareMaterial(material, od, keywordBuilder.keywords);
            keywordBuilder.Assign(material);

            material.SetVector("_Mask", new Vector4(0, 0, 0, occludeDetailWeight));
            Render(od);
        }

        public void InqTreePrototypes(List<TreePrototypeSerializable> prototypes) { }
        public void InqDetailPrototypes(List<DetailPrototypeSerializable> prototypes) { }
#endif

        public void InqTerrainLayers(Terrain terrain, List<TerrainLayer> prototypes) { }
        public bool NeedCurvatureMap() { return filterSet.NeedCurvatureMap(); }


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
                if (filterSet.falloffFilter.filterType != FalloffFilter.FilterType.Global &&
                filterSet.falloffFilter.filterType != FalloffFilter.FilterType.SplineArea)
                {
                    Gizmos.color = MicroVerse.instance.options.colors.occluderStampColor;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), Vector3.one);
                }
            }
        }
#if __MICROVERSE_OBJECTS__
        public void ApplyObjectStamp(ObjectData td, Dictionary<Terrain, List<ObjectJobHolder>> jobs, OcclusionData od)
        {
            if (occludeObjectWeight <= 0)
            {
                return;
            }
            keywordBuilder.Clear();
            PrepareMaterial(material, od, keywordBuilder.keywords);
            keywordBuilder.Assign(material);

            material.SetVector("_Mask", new Vector4(occludeObjectWeight, 0, 0, 0));
            RenderTexture temp = RenderTexture.GetTemporary(od.objectMask.descriptor);
            temp.name = "Occlusion::Render::ObjectMaskTemp";
            material.SetTexture("_MainTex", od.objectMask);
            Graphics.Blit(od.objectMask, temp, material);
            RenderTexture.ReleaseTemporary(od.objectMask);
            od.objectMask = temp;
        }

        public void ProcessObjectStamp(ObjectData td, Dictionary<Terrain, List<ObjectJobHolder>> jobs, OcclusionData od)
        {
            
        }

        public void ApplyObjectClear(ObjectData td)
        {
            
        }

        public bool NeedObjectClear()
        {
            return false;
        }
#endif
    }
}