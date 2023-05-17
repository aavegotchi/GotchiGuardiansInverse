using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

#if __MICROVERSE_SPLINES__
using UnityEngine.Splines;
#endif

namespace JBooth.MicroVerseCore
{
    public partial class AmbientArea : MonoBehaviour
    {
        public enum AmbianceFalloff
        {
            Global,
            Box,
            Range,
            Spline,
            //SplineArea
        }

        [Tooltip("Ambient sound configuration for area")]
        public Ambient ambient;

        public AmbianceFalloff falloff = AmbianceFalloff.Range;
        public Vector2 falloffParams = new Vector2(0.8f, 1.0f);
#if __MICROVERSE_SPLINES__
        public SplineContainer spline = null;
#endif

        AmbientState ambientState;
        ClipPlayer clipPlayer;



        private void OnEnable()
        {
            AmbianceMgr.EnsureExists();
            AmbianceMgr.RegisterArea(this);
            if (ambient != null && ambient.randomSounds.Length > 0)
            {
                ambientState = new AmbientState(ambient);
            }
            if (ambient != null && ambient.backgroundLoops.Length > 0)
            {
                clipPlayer = new ClipPlayer(ambient.backgroundLoops, ambient.outputGroup, ClipPlayer.PlayOrder.Random);
            }
        }

        private void OnDisable()
        {
            AmbianceMgr.UnregisterArea(this);
            ambientState = null;
            clipPlayer = null;
        }
        bool PointInsidePolygon(Vector2 point, List<Vector2> polygon)
        {
            Vector2 p1, p2;
            p1 = polygon[0];
            var counter = 0;
            for (int i = 1; i <= polygon.Count; i++)
            {
                p2 = polygon[i % polygon.Count];
                if (point.y > Mathf.Min(p1.y, p2.y))
                {
                    if (point.y <= Mathf.Max(p1.y, p2.y))
                    {
                        if (point.x <= Mathf.Max(p1.x, p2.x))
                        {
                            if (p1.y != p2.y)
                            {
                                var xinters = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                                if (p1.x == p2.x || point.x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            if (counter % 2 == 0)
                return false;
            else
                return true;
        }

        float GetFalloff(Vector3 worldPos)
        {
            switch (falloff)
            {
                case AmbianceFalloff.Global:
                    return 1;
                case AmbianceFalloff.Box:
                    {
                        float3 objPos = transform.worldToLocalMatrix.MultiplyPoint(worldPos);
                        objPos = math.saturate(objPos);
                        objPos -= 0.5f;
                        objPos = math.abs(objPos);
                        objPos = 0.5f - objPos;
                        float fo = 1.0f - falloffParams.y;
                        objPos = math.smoothstep(objPos, 0, 0.03f * fo);
                        return math.min(objPos.x, objPos.y);
                    }
                case AmbianceFalloff.Range:
                    {
                        Vector3 objPos = transform.worldToLocalMatrix.MultiplyPoint(worldPos);
                        float dist = Vector3.Distance(objPos, Vector3.zero);
                        return 1.0f - Mathf.Clamp01((dist - falloffParams.x) / math.max(0.001f, falloffParams.y - falloffParams.x));
                    }
#if __MICROVERSE_SPLINES__
                case AmbianceFalloff.Spline:
                    {
                        if (spline != null)
                        {
                            using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
                            float d = SplineUtility.GetNearestPoint(native, worldPos, out float3 p, out float t);
                            if (d <= falloffParams.x)
                            {
                                return 1;
                            }
                            else if (falloffParams.y > 0 && d <= falloffParams.x + falloffParams.y)
                            {
                                return 1.0f - Mathf.Clamp01((d - falloffParams.x) / math.max(0.001f, falloffParams.y - falloffParams.x));
                            }
                        }
                        return 0;
                    }
                    /*
                case AmbianceFalloff.SplineArea:
                    {
                        if (spline != null)
                        {
                           
                            using var native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
                            float d = SplineUtility.GetNearestPoint(native, worldPos, out float3 p, out float t);
                            float3 tangent = SplineUtility.EvaluateTangent(native, t);
                            float3 dir = new float3(worldPos.x, worldPos.y, worldPos.z) - p;
                            float3 tangRot = Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0)).MultiplyVector(tangent);
                            float dot = math.dot(dir, tangRot);
                            if (dot < 0)
                                return 0;
                            return math.saturate(d/falloffParams.y);
                        }
                        return 0;
                    }
                    */
#endif

            }

            return 0;
        }

        internal float audioChance;

        internal void UpdateArea(Vector3 listenerPos)
        {
            audioChance = GetFalloff(listenerPos);
            if (ambientState != null)
            {
                ambientState.UpdateState(this, listenerPos);
            }
            if (clipPlayer != null)
            {
                clipPlayer.UpdatePlayer(audioChance * ambient.backgroundVolume * AmbianceMgr.ambientLevel);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (MicroVerse.instance != null)
            {
                Gizmos.color = MicroVerse.instance.options.colors.ambientAreaColor;
                Vector3 size = transform.lossyScale;
                if (falloff == AmbianceFalloff.Range)
                {
                    var old = Gizmos.matrix;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawSphere(Vector3.zero, 0.5f);
                    Gizmos.matrix = old;
                }
                else if (falloff == AmbianceFalloff.Box)
                {
                    Gizmos.DrawCube(transform.position, size);
                }
            }
        }

#endif
    }
}