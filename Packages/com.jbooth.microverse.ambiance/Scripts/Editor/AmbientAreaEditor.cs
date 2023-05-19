using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JBooth.MicroVerseCore
{
    [CustomEditor(typeof(AmbientArea))]
    public class AmbientAreaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ambient"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("falloff"));
            AmbientArea aa = (target as AmbientArea);
            Vector2 falloffParams = aa.falloffParams;
            EditorGUI.BeginChangeCheck();
            switch (aa.falloff)
            {
                case AmbientArea.AmbianceFalloff.Box:
                    falloffParams.y = EditorGUILayout.Slider("Falloff", falloffParams.y, 0, 1);
                    break;
                case AmbientArea.AmbianceFalloff.Range:
                    Vector2 fp = new Vector2(falloffParams.x, falloffParams.y);
                    fp = GUIUtil.DrawMinMax("Range", fp, new Vector2(0, 1));
                    falloffParams.x = fp.x;
                    falloffParams.y = fp.y;
                    break;
                
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.FindProperty("falloffParams").vector2Value = falloffParams;
            }


#if __MICROVERSE_SPLINES__
            if (aa.falloff == AmbientArea.AmbianceFalloff.Spline) // || aa.falloff == AmbientArea.AmbianceFalloff.SplineArea)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spline"));
                EditorGUI.BeginChangeCheck();

                Vector2 fp = new Vector2(falloffParams.x, falloffParams.y);
                fp = EditorGUILayout.Vector2Field("Range", fp);
                falloffParams.x = fp.x;
                falloffParams.y = fp.y;


                
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.FindProperty("falloffParams").vector2Value = falloffParams;
                }
            }
#endif
            serializedObject.ApplyModifiedProperties();
        }

    }
}
