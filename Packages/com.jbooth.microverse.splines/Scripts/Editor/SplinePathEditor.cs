using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_2021_3_OR_NEWER

using UnityEngine.Splines;
using UnityEditor.Splines;

namespace JBooth.MicroVerseCore
{
    [CustomEditor(typeof(SplinePath), true)]
    [CanEditMultipleObjects]
    class SplinePathEditor : Editor
    {
        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            if (MicroVerse.instance)
            {
                MicroVerse.instance.RequestHeightSaveback();
            }

            EditorSplineUtility.RegisterSplineDataChanged<float>(OnAfterSplineDataWasModified);
            Spline.Changed += OnSplineChanged;
        }

        private void OnSplineChanged(Spline spline, int arg2, SplineModification arg3)
        {
            var path = target as SplinePath;
            if (MicroVerse.instance == null) return;
            if (!MicroVerse.instance.enabled) return;
            if (!path.enabled) return;

            if (path.spline != null)
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

        
        void OnAfterSplineDataWasModified(SplineData<float> splineData)
        {
            var path = target as SplinePath;
       
            if (!MicroVerse.instance.enabled) return;
            if (!path.enabled) return;

            foreach (var sw in path.splineWidths)
            {
                if (splineData == sw.widthData)
                {
                    path.UpdateSplineSDFs();
                    EditorUtility.SetDirty(path);
                    MicroVerse.instance?.Invalidate();
                }
            }
        }

        private void OnSceneGUI()
        {
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                MicroVerse.instance?.RequestHeightSaveback();
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            if (MicroVerse.instance)
            {
                MicroVerse.instance.RequestHeightSaveback();
            }
            EditorSplineUtility.UnregisterSplineDataChanged<float>(OnAfterSplineDataWasModified);
            Spline.Changed += OnSplineChanged;

        }

        private void OnUpdate()
        {
            foreach (var target in targets)
            {
                var path = (SplinePath)target;
                if (path != null && path.transform.hasChanged)
                {
                    path.UpdateSplineSDFs();
                    path.transform.hasChanged = false;
                    MicroVerse.instance?.Invalidate();
                }
            }
        }
        static GUIContent CWidthEasing = new GUIContent("Width Easing", "Controls the easing curve for the width of the spline when not consistent");
        public override void OnInspectorGUI()
        {
            GUIUtil.DrawHeaderLogo();
            using var changeScope = new EditorGUI.ChangeCheckScope();
            SplinePath sp = (SplinePath)target;
            if (sp.GetComponentInParent<MicroVerse>() == null)
            {
                EditorGUILayout.HelpBox("Stamp is not under MicroVerse in the heriarchy, will have no effect", MessageType.Warning);
            }
            serializedObject.Update();
            if (sp.multiSpline == null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spline"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("treatAsSplineArea"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sdfRes"));
            using (new GUILayout.VerticalScope(GUIUtil.boxStyle))
            {
                var hprop = serializedObject.FindProperty("modifyHeightMap");
                EditorGUILayout.PropertyField(hprop);
                if (hprop.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothness"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("trench"));
                    GUIUtil.DrawNoise(sp, sp.heightNoise, "Height Noise");
                    EditorGUILayout.Space();
                    using (new GUILayout.VerticalScope(GUIUtil.boxStyle))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("embankmentEasing").FindPropertyRelative("blend"), new GUIContent("Embankment Easing"));
                        GUIUtil.DrawNoise(sp, sp.embankmentNoise, "Embankment Noise");
                    }
                }
                EditorGUI.indentLevel--;

                
            }

            using (new GUILayout.VerticalScope(GUIUtil.boxStyle))
            {
                var sprop = serializedObject.FindProperty("modifySplatMap");
                EditorGUILayout.PropertyField(sprop);
                if (sprop.boolValue)
                {
                    EditorGUI.indentLevel++;
                    GUIUtil.DrawTextureLayerSelector(serializedObject.FindProperty("layer"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("splatWeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("splatWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("splatSmoothness"));
                    GUIUtil.DrawNoise(sp, sp.splatNoise);
                    GUIUtil.DrawTextureLayerSelector(serializedObject.FindProperty("embankmentLayer"));
                    EditorGUI.indentLevel--;
                }

                
            }
            using (new GUILayout.VerticalScope(GUIUtil.boxStyle))
            {
                var ocm = serializedObject.FindProperty("occludeHeightMod");
                EditorGUILayout.PropertyField(ocm);
                if (ocm.boolValue != false)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("occludeHeightWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("occludeHeightSmoothness"));
                    EditorGUI.indentLevel--;
                }
                var oct = serializedObject.FindProperty("occludeTextureMod");
                EditorGUILayout.PropertyField(oct);
                if (oct.boolValue != false)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("occludeTextureWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("occludeTextureSmoothness"));
                    EditorGUI.indentLevel--;
                }
                var clearTrees = serializedObject.FindProperty("clearTrees");
                EditorGUILayout.PropertyField(clearTrees);
                if (clearTrees.boolValue != false)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("treeWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("treeSmoothness"));
                    EditorGUI.indentLevel--;
                }
                var clearDetails = serializedObject.FindProperty("clearDetails");
                EditorGUILayout.PropertyField(clearDetails);
                if (clearDetails.boolValue != false)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("detailWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("detailSmoothness"));
                    EditorGUI.indentLevel--;
                }
                var clearObjects = serializedObject.FindProperty("clearObjects");
                EditorGUILayout.PropertyField(clearObjects);
                if (clearObjects.boolValue != false)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("objectWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("objectSmoothness"));
                    EditorGUI.indentLevel--;
                }
            }
            using (new GUILayout.VerticalScope(GUIUtil.boxStyle))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("splineWidthEasing").FindPropertyRelative("blend"), CWidthEasing);
            }
            serializedObject.ApplyModifiedProperties();
            if (changeScope.changed)
            {
                sp.UpdateSplineSDFs();
                MicroVerse.instance?.Invalidate();
            }
            EditorGUILayout.BeginHorizontal();
         
            if (GUILayout.Button("Add Objects along Spline"))
            {
                foreach (var target in targets)
                {
                    SplinePath sps = (SplinePath)target;
                    sps.gameObject.AddComponent<SplineInstantiate>();
                }
            }

            /*
            GUI.enabled = false;
            if (GUILayout.Button("Add Spline Mesh"))
            {
                foreach (var target in targets)
                {
                    SplinePath sps = (SplinePath)target;
                    //sps.gameObject.AddComponent<SplineMesh>();
                }
            }
       
            GUI.enabled = true;
            */
            EditorGUILayout.EndHorizontal();
        }
    }

}
#endif

