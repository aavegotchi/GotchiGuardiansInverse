using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2021_3_OR_NEWER

using UnityEngine.Splines;
using UnityEditor.Splines;

namespace JBooth.MicroVerseCore
{
    [CustomEditor(typeof(SplineArea), true)]
    [CanEditMultipleObjects]
    public class SplineAreaEditor : Editor
    {
        private void OnEnable()
        {
            EditorSplineUtility.AfterSplineWasModified += OnAfterSplineWasModified;
        }
        private void OnDisable()
        {
            EditorSplineUtility.AfterSplineWasModified -= OnAfterSplineWasModified;
        }

        void OnAfterSplineWasModified(Spline spline)
        {
            var path = target as SplineArea;
            if (path != null && path.spline != null && path.spline.Splines != null)
            {
                foreach (var s in path.spline.Splines)
                {
                    if (ReferenceEquals(spline, s))
                    {
                        path.UpdateSplineSDFs();
                        MicroVerse.instance?.Invalidate();
                        return;
                    }
                }
            }
        }
    }
}
#endif