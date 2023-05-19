using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JBooth.MicroVerseCore
{
    public class TerrainUtil
    {
        public static Bounds ComputeTerrainBounds(Terrain terrain)
        {
            var terrainBounds = terrain.terrainData.bounds;
            terrainBounds.center = terrain.transform.position;
            terrainBounds.center += new Vector3(terrainBounds.size.x * 0.5f, 0, terrainBounds.size.z * 0.5f);
            return terrainBounds;
        }

        /// <summary>
        /// Compute the total bounds of the provided terrains
        /// </summary>
        /// <param name="terrains"></param>
        /// <returns></returns>
        public static Bounds ComputeTerrainBounds(Terrain[] terrains)
        {
            Bounds terrainBounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < terrains.Length; i++)
            {
                Terrain terrain = terrains[i];
                Bounds terrainWorldBounds = ComputeTerrainBounds(terrain);

                if (i == 0)
                {
                    terrainBounds = terrainWorldBounds;
                }
                else
                {
                    terrainBounds.Encapsulate(terrainWorldBounds);
                }
            }

            return terrainBounds;
        }

        public static Bounds AdjustForRotation(Bounds b, Quaternion rot)
        {
            var mtx = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
            b.Encapsulate(mtx.MultiplyPoint(b.center + b.size / 2));
            b.Encapsulate(mtx.MultiplyPoint(b.center - b.size / 2));
            return b;
        }

        public static Bounds GetBounds(Transform transform)
        {
            Vector3 scale = transform.lossyScale;
            float max = Mathf.Max(scale.x, scale.z);
            var b = TerrainUtil.AdjustForRotation(new Bounds(transform.position, new Vector3(max, max, max)), transform.rotation);
            b.max = new Vector3(b.max.x, 99999, b.max.z);
            b.min = new Vector3(b.min.x, -99999, b.min.z);
            return b;

        }

        public static Vector3 ComputeTerrainSize(Terrain terrain)
        {
            return new Vector3(
                terrain.terrainData.heightmapScale.x * (terrain.terrainData.heightmapResolution),
                terrain.terrainData.heightmapScale.y * 2,
                terrain.terrainData.heightmapScale.z * (terrain.terrainData.heightmapResolution));
        }

        public static Matrix4x4 ComputeStampMatrix(Terrain terrain, Transform transform, bool heightStamp = false)
        {
            Vector2 realSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
            //terrain.terrainData.heightmapScale.x * (terrain.terrainData.heightmapResolution),
            //terrain.terrainData.heightmapScale.z * (terrain.terrainData.heightmapResolution)) ;
            if (heightStamp)
            {
                realSize = new Vector2(
                terrain.terrainData.heightmapScale.x * (terrain.terrainData.heightmapResolution),
                terrain.terrainData.heightmapScale.z * (terrain.terrainData.heightmapResolution));
            }

            var localPosition = terrain.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
            var size = transform.lossyScale;
            Vector2 size2D = new Vector2(size.x, size.z);
            var pos = new Vector2(localPosition.x, localPosition.z);
     
            var pos01 = pos / realSize;
            var rotation = transform.rotation.eulerAngles.y;
            var m = Matrix4x4.Translate(-pos01);
            m = Matrix4x4.Rotate(Quaternion.AngleAxis(rotation, Vector3.forward)) * m;
            m = Matrix4x4.Scale(realSize / size2D) * m;
            m = Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0)) * m;
            return m;
        }

        public static int FindTextureChannelIndex(Terrain terrain, TerrainLayer layer)
        {
            var layers = terrain.terrainData.terrainLayers;
            for (var index = 0; index < layers.Length; index++)
            {
                var l = layers[index];
                if (!ReferenceEquals(l, layer))
                    continue;
                return index;
            }

            return -1;
        }

        public static int FindTreeIndex(Terrain terrain, GameObject prefab)
        {
            var protos = terrain.terrainData.treePrototypes;
            for (var index = 0; index < protos.Length; index++)
            {
                var l = protos[index];
                if (!ReferenceEquals(l.prefab, prefab))
                    continue;
                return index;
            }

            return -1;
        }


        public static void EnsureTexturesAreOnTerrain(Terrain terrain, List<TerrainLayer> prototypes)
        {
            var terrainLayers = terrain.terrainData.terrainLayers;
            List<TerrainLayer> resultLayers = new List<TerrainLayer>(terrainLayers);
            bool edited = false;
            int index = -1;
            foreach (var prototype in prototypes.Distinct())
            {
                for (int i = 0; i < terrainLayers.Length; ++i)
                {
                    var tp = terrainLayers[i];
                    if (ReferenceEquals(prototype, tp))
                    {
                        index = i;
                    }
                }
                if (index < 0)
                {
                    resultLayers.Add(prototype);
                    
                    edited = true;

                }
            }
            if (edited)
            {
                terrain.terrainData.terrainLayers = resultLayers.ToArray();
            }
        }


    }
}
