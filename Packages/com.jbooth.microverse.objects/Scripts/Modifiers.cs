using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    public interface IObjectModifier : ISpawner
    {
        void ApplyObjectStamp(ObjectData td, Dictionary<Terrain, List<ObjectJobHolder>> jobs, OcclusionData od);
        void ProcessObjectStamp(ObjectData td, Dictionary<Terrain, List<ObjectJobHolder>> jobs, OcclusionData od);
        void ApplyObjectClear(ObjectData td);
        bool NeedObjectClear();
        bool NeedCurvatureMap();
        bool OccludesOthers();
        bool NeedSDF();
    }

    public class ObjectData : StampData
    {
        public RenderTexture heightMap;
        public RenderTexture normalMap;
        public RenderTexture curveMap;
        public RenderTexture indexMap;
        public RenderTexture weightMap;
        public RenderTexture clearMap;
        public int layerIndex = 0;

        public ObjectData(Terrain terrain,
            RenderTexture height,
            RenderTexture normal,
            RenderTexture curve,
            RenderTexture indexMap,
            RenderTexture weightMap,
            RenderTexture clearMap) : base(terrain)
        {
            this.terrain = terrain;
            heightMap = height;
            normalMap = normal;
            curveMap = curve;
            this.indexMap = indexMap;
            this.weightMap = weightMap;
            this.clearMap = clearMap;
        }
    }
}