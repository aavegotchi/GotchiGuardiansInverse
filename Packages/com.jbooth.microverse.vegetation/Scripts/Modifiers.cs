using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    public interface ITreeModifier : ISpawner
    {
        bool NeedTreeClear();
        void ApplyTreeClear(TreeData td);
        void ApplyTreeStamp(TreeData vd, Dictionary<Terrain, List<TreeJobHolder>> jobs, OcclusionData od);
        void ProcessTreeStamp(TreeData vd, Dictionary<Terrain, List<TreeJobHolder>> jobs, OcclusionData od);
        void InqTreePrototypes(List<TreePrototypeSerializable> prototypes);
        bool NeedCurvatureMap();
        bool OccludesOthers();
        bool NeedSDF();
    }

    public class TreeData : StampData
    {
        public RenderTexture heightMap;
        public RenderTexture normalMap;
        public RenderTexture curveMap;
        public RenderTexture treeClearMap;
        public MicroVerse.DataCache dataCache;

        public int layerIndex = 0;

        public TreeData(Terrain terrain,
            RenderTexture height,
            RenderTexture normal,
            RenderTexture curve,
            RenderTexture clearMap,
            MicroVerse.DataCache dataCache) : base(terrain)
        {
            this.terrain = terrain;
            heightMap = height;
            normalMap = normal;
            curveMap = curve;
            this.dataCache = dataCache;
            this.treeClearMap = clearMap;
        }
    }


    public interface IDetailModifier : ISpawner
    {
        bool NeedDetailClear();
        void ApplyDetailClear(DetailData td);
        void ApplyDetailStamp(DetailData dd, Dictionary<Terrain, Dictionary<int, List<RenderTexture>>> resultBuffers, OcclusionData od);
        void InqDetailPrototypes(List<DetailPrototypeSerializable> prototypes);
        bool NeedCurvatureMap();
        bool NeedSDF();

    }


    public class DetailData : StampData
    {
        public RenderTexture heightMap;
        public RenderTexture normalMap;
        public RenderTexture curveMap;
        public RenderTexture clearMap;
        public MicroVerse.DataCache dataCache;
        public int layerIndex = 0;

        public DetailData(Terrain terrain,
            RenderTexture height,
            RenderTexture normal,
            RenderTexture curve,
            RenderTexture clearMap,
            MicroVerse.DataCache dataCache) : base(terrain)
        {
            this.terrain = terrain;
            heightMap = height;
            normalMap = normal;
            curveMap = curve;
            this.dataCache = dataCache;
            this.clearMap = clearMap;
        }
    }
}