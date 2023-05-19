
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

namespace JBooth.MicroVerseCore
{
    public class MapGen
    {
        static Shader curvatureShader = null;
        public static RenderTexture GenerateCurvatureMap(Terrain t, Dictionary<Terrain, RenderTexture> normals, int width, int height)
        {
            Profiler.BeginSample("Generate Curvature Map");
            if (curvatureShader == null)
            {
                curvatureShader = Shader.Find("Hidden/MicroVerse/CurvatureMapGen");
            }
            var material = new Material(curvatureShader);
            var desc = new RenderTextureDescriptor(width, height, 0);
            desc.colorFormat = RenderTextureFormat.R8;
            desc.useMipMap = true;
            var rt = RenderTexture.GetTemporary(desc);
            rt.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = rt;
            material.SetTexture("_Normalmap", normals[t]);
            
            if (t.leftNeighbor && normals.ContainsKey(t.leftNeighbor))
            {
                material.SetTexture("_Normalmap_NX", normals[t.leftNeighbor]);
                material.EnableKeyword("_NX");
            }
            if (t.rightNeighbor && normals.ContainsKey(t.rightNeighbor))
            {
                material.SetTexture("_Normalmap_PX", normals[t.rightNeighbor]);
                material.EnableKeyword("_PX");
            }
            if (t.bottomNeighbor && normals.ContainsKey(t.bottomNeighbor))
            {
                material.SetTexture("_Normalmap_NY", normals[t.bottomNeighbor]);
                material.EnableKeyword("_NY");
            }
            if (t.topNeighbor && normals.ContainsKey(t.topNeighbor))
            {
                material.SetTexture("_Normalmap_PY", normals[t.topNeighbor]);
                material.EnableKeyword("_PY");
            }

            Graphics.Blit(null, rt, material);
            GameObject.DestroyImmediate(material);
            Profiler.EndSample();
            return rt;
        }


        static Shader normalShader = null;
        // because Unity defers normal map generation until it's too late
        public static RenderTexture GenerateNormalMap(Terrain t, Dictionary<Terrain, RenderTexture> heightMaps, int width, int height)
        {
            Profiler.BeginSample("Generate Normal");
            if (normalShader == null)
            {
                normalShader = Shader.Find("Hidden/MicroVerse/NormalMapGen");
            }
            var material = new Material(normalShader);
            var desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
            desc.useMipMap = true;
            var rt = RenderTexture.GetTemporary(desc);
            rt.wrapMode = TextureWrapMode.Clamp;
            RenderTexture.active = rt;
            material.SetTexture("_Heightmap", heightMaps[t]);
            if (t.rightNeighbor && heightMaps.ContainsKey(t.rightNeighbor))
            { 
                material.SetTexture("_Heightmap_PX", heightMaps[t.rightNeighbor]);
                material.SetKeyword(new UnityEngine.Rendering.LocalKeyword(material.shader, "_PX"), true);
            }
            if (t.topNeighbor && heightMaps.ContainsKey(t.topNeighbor))
            {
                material.SetTexture("_Heightmap_PY", heightMaps[t.topNeighbor]);
                material.SetKeyword(new UnityEngine.Rendering.LocalKeyword(material.shader, "_PY"), true);
            }

            if (t.leftNeighbor && heightMaps.ContainsKey(t.leftNeighbor))
            {
                material.SetTexture("_Heightmap_NX", heightMaps[t.leftNeighbor]);
                material.EnableKeyword("_NX");
            }

            if (t.bottomNeighbor && heightMaps.ContainsKey(t.bottomNeighbor))
            {
                material.SetTexture("_Heightmap_NY", heightMaps[t.bottomNeighbor]);
                material.EnableKeyword("_NY");
            }

            Graphics.Blit(null, rt, material);
            GameObject.DestroyImmediate(material);
            Profiler.EndSample();
            return rt;
        }
    }
}
