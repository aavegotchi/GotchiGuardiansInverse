using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    [ExecuteAlways]
    public class Stamp : MonoBehaviour
    {
        public class KeywordBuilder
        {
            public List<string> keywords = new List<string>(32);

            public void Add(string k)
            {
                keywords.Add(k);
            }

            public void Clear()
            {
                keywords.Clear();
            }

            public void Assign(Material mat)
            {
                mat.shaderKeywords = keywords.ToArray();
            }

            public void Remove(string k)
            {
                keywords.Remove(k);
            }
        }

        protected static KeywordBuilder keywordBuilder = new KeywordBuilder();

        public void StripInBuild()
        {
            if (Application.isPlaying)
                Destroy(this);
            else
                DestroyImmediate(this);
        }
        public bool IsEnabled() { return gameObject.activeInHierarchy && enabled; }

        public virtual void OnEnable()
        {
#if UNITY_EDITOR
            cachedMtx = transform.localToWorldMatrix;
#endif
            transform.hasChanged = false;
            MicroVerse.instance?.Invalidate();
            MicroVerse.instance?.RequestHeightSaveback();
        }

        public virtual void OnDisable()
        {
            MicroVerse.instance?.Invalidate();
            MicroVerse.instance?.RequestHeightSaveback();
        }

        public virtual FilterSet GetFilterSet()
        {
            return null;
        }
#if UNITY_EDITOR

        public virtual void OnMoved()
        {  
            cachedMtx = transform.localToWorldMatrix;
            MicroVerse.instance?.Invalidate();   
        }

        private void OnDestroy()
        {
            if (MicroVerse.instance != null)
                MicroVerse.instance.Invalidate();
        }

        Matrix4x4 cachedMtx;
        void Update()
        {
            if (cachedMtx != transform.localToWorldMatrix)
            {
                OnMoved();
            }
            
        }
#endif
    }
}
