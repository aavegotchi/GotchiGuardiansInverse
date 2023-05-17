using Unity.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
#if UNITY_2021_3_OR_NEWER

using UnityEngine.Splines;

// SDF spline renderer - convert spline into SDF for fast sampling
namespace JBooth.MicroVerseCore
{
    public class SplineRenderer
    {
        private ComputeBuffer curveBuffer;
        private ComputeBuffer lengthBuffer;
        private ComputeBuffer widthBuffer;
        private Vector4 info;
        private Vector4 widthInfo;

        public RenderTexture splineSDF;

        static Shader slineRenderShader = null;
        static Shader splineClearShader = null;

        public struct RenderDesc
        {
            public SplineContainer splineContainer;
            public List<SplinePath.SplineWidthData> widths;
            public Easing widthEasing;
            public float widthBoost;
            public bool isArea;
        };

        public void Render(SplineContainer sc, Terrain terrain, List<SplinePath.SplineWidthData> widths = null, Easing easing = null, int sdfRes = 512)
        {
            RenderDesc rd = new RenderDesc()
            {
                splineContainer = sc,
                widths = widths,
                widthEasing = easing,
                widthBoost = 0,
                isArea = true,
            };
            Render(new RenderDesc[1] { rd }, terrain, sdfRes);
        }

        public void Render(RenderDesc[] renderDescs, Terrain terrain, int sdfRes = 512)
        {
            
            // allocate main spline texture
            int targetRes = terrain.terrainData.alphamapResolution;
            if (splineSDF != null)
            {
                splineSDF.Release();
                Object.DestroyImmediate(splineSDF);
            }
            splineSDF = new RenderTexture(targetRes, targetRes, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            splineSDF.name = "SplineRenderer::SplineSDFFill";
            splineSDF.wrapMode = TextureWrapMode.Clamp;
            if (slineRenderShader == null)
            {
                slineRenderShader = Shader.Find("Hidden/MicroVerse/SplineSDFFill");
            }
            Material splineSDFMat = new Material(slineRenderShader);
            if (sdfRes > targetRes)
                sdfRes = targetRes;
            RenderTexture rtLargeA = RenderTexture.GetTemporary(targetRes, targetRes, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            RenderTexture rtLargeB = RenderTexture.GetTemporary(targetRes, targetRes, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);


            splineSDFMat.SetVector("_RealSize", TerrainUtil.ComputeTerrainSize(terrain));
            splineSDFMat.SetMatrix("_Transform", terrain.transform.localToWorldMatrix);
            

            Graphics.Blit(Texture2D.blackTexture, splineSDF);
            RenderTexture rtA = RenderTexture.GetTemporary(sdfRes, sdfRes, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            RenderTexture rtB = RenderTexture.GetTemporary(sdfRes, sdfRes, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

            if (splineClearShader == null)
            {
                splineClearShader = Shader.Find("Hidden/MicroVerse/SplineClear");
            }
            var mat = new Material(splineClearShader);
            Graphics.Blit(null, rtA, mat);
            Graphics.Blit(null, rtB, mat);
            Graphics.Blit(null, rtLargeA, mat);
            Graphics.Blit(null, rtLargeB, mat);
            GameObject.DestroyImmediate(mat);

            foreach (var desc in renderDescs)
            {
                int splineIdx = -1;
                // do at low res first
                var spline = desc.splineContainer;
                var widths = desc.widths;
                var widthEasing = desc.widthEasing;
                float widthBoost = desc.widthBoost;
                if (spline == null)
                    continue;
                List<string> keywords = new List<string>(32);
                splineSDFMat.SetInt("_IsArea", desc.isArea ? 1 : 0);
                foreach (var splineSpline in spline.Splines)
                {
                    keywords.Clear();
                    splineIdx++;
                    var knotCount = splineSpline.Count;
                    if (knotCount < 2)
                        continue;

                    UnityEngine.Profiling.Profiler.BeginSample("Spline To SDF Rendering");

                    if (curveBuffer != null)
                        curveBuffer.Dispose();
                    if (lengthBuffer != null)
                        lengthBuffer.Dispose();
                    if (widthBuffer != null)
                        widthBuffer.Dispose();
                    widthBuffer = null;
                    widthInfo = Vector4.zero;
                    if (widths != null)
                    {
                        if (splineIdx < widths.Count)
                        {
                            var w = widths[splineIdx].widthData;
                            if (w.Count > 0)
                            {
                                w.SortIfNecessary();
                                w.ConvertPathUnit(splineSpline, PathIndexUnit.Knot);
                                widthBuffer = new ComputeBuffer(w.Count, UnsafeUtility.SizeOf<Vector2>());
                                var wn = new NativeArray<Vector2>(w.Count, Allocator.Temp);
                                for (int i = 0; i < w.Count; ++i)
                                {

                                    wn[i] = new Vector2(w[i].Index, w[i].Value);
                                }
                                widthBuffer.SetData(wn);
                                widthInfo.x = w.Count;
                                wn.Dispose();
                                if (widthEasing != null)
                                {
                                    widthEasing.PrepareMaterial(splineSDFMat, "_WIDTH", keywords);
                                }
                                w.ConvertPathUnit(splineSpline, PathIndexUnit.Normalized);
                            }
                        }
                    }
                    if (widthBuffer == null)
                    {
                        widthBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<Vector2>());
                    }

                    curveBuffer = new ComputeBuffer(knotCount, UnsafeUtility.SizeOf<BezierCurve>());
                    lengthBuffer = new ComputeBuffer(knotCount, sizeof(float));

                    var curves = new NativeArray<BezierCurve>(knotCount, Allocator.Temp);
                    var lengths = new NativeArray<float>(knotCount, Allocator.Temp);
                    var smtx = spline.transform.localToWorldMatrix;

                    for (int i = 0; i < knotCount; ++i)
                    {
                        var curve = splineSpline.GetCurve(i);
                        // to world space
                        curve.P0 = smtx.MultiplyPoint(curve.P0);
                        curve.P1 = smtx.MultiplyPoint(curve.P1);
                        curve.P2 = smtx.MultiplyPoint(curve.P2);
                        curve.P3 = smtx.MultiplyPoint(curve.P3);

                        curves[i] = curve;
                        lengths[i] = splineSpline.GetCurveLength(i);
                    }

                    curveBuffer.SetData(curves);
                    lengthBuffer.SetData(lengths);

                    curves.Dispose();
                    lengths.Dispose();


                    info = new Vector4(splineSpline.Count, splineSpline.Closed ? 1 : 0, splineSpline.GetLength(), 0);
                    splineSDFMat.SetVector("_Info", info);
                    splineSDFMat.SetVector("_WidthInfo", widthInfo);
                    splineSDFMat.SetBuffer("_Curves", curveBuffer);
                    splineSDFMat.SetBuffer("_CurveLengths", lengthBuffer);
                    splineSDFMat.SetFloat("_WidthBoost", widthBoost);
                    if (widthBuffer != null)
                        splineSDFMat.SetBuffer("_Widths", widthBuffer);

                    splineSDFMat.shaderKeywords = keywords.ToArray();
                    Graphics.Blit(rtB, rtA, splineSDFMat);

                    (rtA, rtB) = (rtB, rtA);

                    keywords.Add("_EDGES");
                    splineSDFMat.shaderKeywords = keywords.ToArray();

                    splineSDFMat.SetTexture("_Prev", rtB);
                    Graphics.Blit(rtLargeB, rtLargeA, splineSDFMat);
                    (rtLargeA, rtLargeB) = (rtLargeB, rtLargeA);

                    curveBuffer.Dispose();
                    lengthBuffer.Dispose();
                    widthBuffer.Dispose();

                    UnityEngine.Profiling.Profiler.EndSample();
                }
            }
            Graphics.Blit(rtLargeB, splineSDF);
            Object.DestroyImmediate(splineSDFMat);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rtA);
            RenderTexture.ReleaseTemporary(rtB);
            RenderTexture.ReleaseTemporary(rtLargeA);
            RenderTexture.ReleaseTemporary(rtLargeB);
        }

        public void Dispose()
        {
            if (splineSDF)
            {
                RenderTexture.active = null;
                splineSDF.Release();
                Object.DestroyImmediate(splineSDF);
                splineSDF = null;
            }
        }

    }
}

#endif