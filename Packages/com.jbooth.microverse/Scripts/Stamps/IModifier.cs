using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    public interface IModifier
    {
        void Initialize(Terrain[] terrains);
        void Dispose();
        Bounds GetBounds();
        bool IsEnabled();
        void StripInBuild();
    }

    public interface ISpawner : IModifier
    {
        bool UsesOtherTreeSDF();
        bool UsesOtherObjectSDF();
        bool NeedParentSDF();
        bool NeedToGenerateSDFForChilden();
        void SetSDF(Terrain t, RenderTexture rt);
        RenderTexture GetSDF(Terrain t);
    }


    public interface IHeightModifier : IModifier
    {
        bool ApplyHeightStamp(RenderTexture source, RenderTexture dest, HeightmapData heightmapData, OcclusionData od);
    }

    public class HeightmapData : StampData
    {
        public HeightmapData(Terrain terrain) : base(terrain)
        {
            this.terrain = terrain;
        }
    }

    public interface IHoleModifier : IModifier
    {
        void ApplyHoleStamp(RenderTexture src, RenderTexture dest,
            HoleData holeData, OcclusionData od);

        bool IsValidHoleStamp();

        bool NeedCurvatureMap();
    }

    public class HoleData : StampData
    {
        public RenderTexture heightMap;
        public RenderTexture normalMap;
        public RenderTexture curveMap;
        public RenderTexture placementMask;
        public RenderTexture indexMap;
        public RenderTexture weightMap;


        public HoleData(Terrain terrain, RenderTexture heightMap,
            RenderTexture normalMap, RenderTexture curveMap,
            RenderTexture indexMap, RenderTexture weightMap) : base(terrain)
        {
            this.heightMap = heightMap;
            this.normalMap = normalMap;
            this.curveMap = curveMap;
            this.indexMap = indexMap;
            this.weightMap = weightMap;
            this.terrain = terrain;
        }

    }

    public interface ITextureModifier : IModifier
    {
        bool ApplyTextureStamp(RenderTexture indexSrc, RenderTexture indexDest,
            RenderTexture weightSrc, RenderTexture weightDest,
            TextureData splatmapData, OcclusionData od);

        void InqTerrainLayers(Terrain terrain, List<TerrainLayer> prototypes);
        bool NeedCurvatureMap();
    }

    public class TextureData : StampData
    {
        public RenderTexture heightMap;
        public RenderTexture normalMap;
        public RenderTexture curveMap;
        public RenderTexture placementMask;
        public RenderTexture indexMap;
        public RenderTexture weightMap;


        public TextureData(Terrain terrain, int alphamapIndex, RenderTexture heightMap,
            RenderTexture normalMap, RenderTexture curveMap) : base(terrain)
        {
            this.heightMap = heightMap;
            this.normalMap = normalMap;
            this.curveMap = curveMap;
            this.terrain = terrain;
        }
    }

}
