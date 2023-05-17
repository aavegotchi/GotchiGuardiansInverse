using UnityEngine;
using System.Collections.Generic;

namespace JBooth.MicroVerseCore
{
    

    [ExecuteAlways]
    public class HeightStamp : Stamp, IHeightModifier
    {
        public enum CombineMode
        {
            Override = 0,
            Max = 1,
            Min = 2,
            Add = 3,
            Subtract = 4,
            Multiply = 5,
            Average = 6,
            Difference = 7,
            SqrtMultiply = 8,
            Blend = 9,
        }

        public Texture2D stamp;
        public CombineMode mode = CombineMode.Max;

        public FalloffFilter falloff = new FalloffFilter();

        [Tooltip("Twists the stamp around the Y axis")]
        [Range(-90, 90)] public float twist = 0;
        [Tooltip("Erodes the slopes of the terrain")]
        [Range(0, 600)] public float erosion = 0;
        [Tooltip("Controls the scale of the erosion effect")]
        [Range(1, 90)] public float erosionSize = 4;

        [Tooltip("Bends the heights towards the top or bottom")]
        [Range(0.1f, 8.0f)] public float power = 1;
        [Tooltip("Invert the height map")]
        public bool invert;

        [Tooltip("Blend between existing height map and new one")]
        [Range(0, 1)] public float blend = 1;

        public Vector2 remapRange = new Vector2(0, 1);
        public Vector4 scaleOffset = new Vector4(1, 1, 0, 0);
        [Range(-1, 1)] public float tiltX = 0;
        [Range(-1, 1)] public float tiltZ = 0;
        public bool tiltScaleX = false;
        public bool tiltScaleZ = false;
        [Range(0, 6)] public float mipBias = 0;

        public Material material { get; private set; }

        public void Dispose()
        {
            DestroyImmediate(material);
        }

        [SerializeField] int version = 0;
        public override void OnEnable()
        {
            if (version == 0 && mode == CombineMode.Max)
            {
                var pos = transform.position;
                pos.y = 0;
                transform.position = pos;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
            }
            else if (version == 1 && mode != HeightStamp.CombineMode.Override && mode != HeightStamp.CombineMode.Max)
            {
                var pos = transform.position;
                pos.y = 0;
                transform.position = pos;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
            }
            base.OnEnable();
            version = 2;
        }

        static Shader heightmapShader = null;

        public void Initialize(Terrain[] terrains)
        {
            if (stamp != null)
            {
                stamp.wrapMode = TextureWrapMode.Clamp;
            }
            if (heightmapShader == null)
            {
                heightmapShader = Shader.Find("Hidden/MicroVerse/HeightmapStamp");
            }
            material = new Material(heightmapShader);
        }

        public Bounds GetBounds()
        {
            FalloffOverride fo = GetComponentInParent<FalloffOverride>();
            var foType = falloff.filterType;
            var foFilter = falloff;
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
            return TerrainUtil.GetBounds(transform);
        }

        static int _AlphaMapSize = Shader.PropertyToID("_AlphaMapSize");
        static int _PlacementMask = Shader.PropertyToID("_PlacementMask");
        static int _NoiseUV = Shader.PropertyToID("_NoiseUV");
        static int _Invert = Shader.PropertyToID("_Invert");
        static int _Blend = Shader.PropertyToID("_Blend");
        static int _Power = Shader.PropertyToID("_Power");
        static int _Tilt = Shader.PropertyToID("_Tilt");
        static int _TiltScale = Shader.PropertyToID("_TiltScale");

        // used by copy paste stamp
        public bool ApplyHeightStampAbsolute(RenderTexture source, RenderTexture dest, HeightmapData heightmapData, OcclusionData od, Vector2 heightRenorm)
        {
            material.SetVector("_HeightRenorm", heightRenorm);
            keywordBuilder.Clear();
            keywordBuilder.Add("_PASTESTAMP");
            keywordBuilder.Add("_ABSOLUTEHEIGHT");
            PrepareMaterial(material, heightmapData, keywordBuilder.keywords);
            material.SetFloat(_AlphaMapSize, source.width);
            material.SetTexture(_PlacementMask, od.terrainMask);
            var noisePos = heightmapData.terrain.transform.position;
            noisePos.x /= heightmapData.terrain.terrainData.size.x;
            noisePos.z /= heightmapData.terrain.terrainData.size.z;
            material.SetFloat(_Power, 1);
            material.SetFloat(_Blend, 1);
            material.SetFloat(_Invert, 0);

            material.SetVector(_NoiseUV, new Vector2(noisePos.x, noisePos.z));

            keywordBuilder.Assign(material);

            Graphics.Blit(source, dest, material);
            return true;
        }

        public bool ApplyHeightStamp(RenderTexture source, RenderTexture dest, HeightmapData heightmapData, OcclusionData od)
        {
            keywordBuilder.Clear();
            PrepareMaterial(material, heightmapData, keywordBuilder.keywords);
            material.SetFloat(_AlphaMapSize, source.width);
            material.SetTexture(_PlacementMask, od.terrainMask);
            var noisePos = heightmapData.terrain.transform.position;
            noisePos.x /= heightmapData.terrain.terrainData.size.x;
            noisePos.z /= heightmapData.terrain.terrainData.size.z;

            material.SetVector(_NoiseUV, new Vector2(noisePos.x, noisePos.z));
            material.SetFloat(_Power, power);
            material.SetFloat(_Blend, blend);
            material.SetFloat(_Invert, invert ? 1.0f : 0.0f);

            var euler = transform.eulerAngles;
            
            if (euler.x >= 180) euler.x -= 360;
            if (euler.z >= 180) euler.z -= 360;
            euler.x = Mathf.Clamp(euler.x, -90, 90);
            euler.z = Mathf.Clamp(euler.z, -90, 90);

            material.SetVector(_TiltScale, new Vector2(tiltScaleX ? 1 : 0, tiltScaleZ ? 1 : 0));
            material.SetVector(_Tilt, new Vector3(tiltX, 0, tiltZ));

            keywordBuilder.Assign(material);

            Graphics.Blit(source, dest, material);
            return true;
        }

        static int _Transform = Shader.PropertyToID("_Transform");
        static int _RealSize = Shader.PropertyToID("_RealSize");
        static int _StampTex = Shader.PropertyToID("_StampTex");
        static int _MipBias = Shader.PropertyToID("_MipBias");
        static int _RemapRange = Shader.PropertyToID("_RemapRange");
        static int _ScaleOffset = Shader.PropertyToID("_ScaleOffset");
        static int _HeightRemap = Shader.PropertyToID("_HeightRemap");
        static int _CombineMode = Shader.PropertyToID("_CombineMode");
        static int _Twist = Shader.PropertyToID("_Twist");
        static int _Erosion = Shader.PropertyToID("_Erosion");
        static int _ErosionSize = Shader.PropertyToID("_ErosionSize");

        void PrepareMaterial(Material material, HeightmapData heightmapData, List<string> keywords)
        {
            var localPosition = heightmapData.WorldToTerrainMatrix.MultiplyPoint3x4(transform.position);
            var size = transform.lossyScale;

            material.SetMatrix(_Transform, TerrainUtil.ComputeStampMatrix(heightmapData.terrain, transform, true));
            material.SetVector(_RealSize, TerrainUtil.ComputeTerrainSize(heightmapData.terrain));
            
            if (stamp != null)
            {
                stamp.wrapMode = (scaleOffset == new Vector4(1,1,0,0)) ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;
            }
            material.SetTexture(_StampTex, stamp);
            material.SetFloat(_MipBias, mipBias);
            material.SetVector(_RemapRange, remapRange);
            material.SetVector(_ScaleOffset, scaleOffset);
            
            falloff.PrepareMaterial(material, heightmapData.terrain, transform, keywords);
           

            var y = localPosition.y;

            material.SetVector(_HeightRemap, new Vector2(y, y + size.y) / heightmapData.RealHeight);
            material.SetInt(_CombineMode, (int)mode);

            if (twist != 0)
            {
                keywords.Add("_TWIST");
                material.SetFloat(_Twist, twist);
            }
            if (erosion != 0)
            {
                keywords.Add("_EROSION");
                material.SetFloat(_Erosion, erosion);
                material.SetFloat(_ErosionSize, erosionSize);
            }
            

        }

        void OnDrawGizmosSelected()
        {
            if (MicroVerse.instance != null)
            {
                Gizmos.color = MicroVerse.instance.options.colors.heightStampColor;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), Vector3.one);
            }
        }
    }
}