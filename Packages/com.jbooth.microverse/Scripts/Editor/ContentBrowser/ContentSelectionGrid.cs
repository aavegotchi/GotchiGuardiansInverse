using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static JBooth.MicroVerseCore.Browser.ContentBrowser;

namespace JBooth.MicroVerseCore.Browser
{

    /// <summary>
    /// Draw the contents of selected collection. Support dragging into the scene view
    /// </summary>
    public class ContentSelectionGrid
    {
        private ContentBrowser contentBrowser;

        private SceneDragHandler sceneDragHandler;

        private static int cellSize = 96;
        private const int infoWidth = 240;

        private Vector2 contentScrollPosition = Vector2.zero;

        private PresetItem selectedPresetItem = null;

        public ContentSelectionGrid(ContentBrowser contentBrowser)
        {
            this.contentBrowser = contentBrowser;
        }

        public void OnEnable()
        {
            // scene drag handler
            if (sceneDragHandler == null)
            {
                sceneDragHandler = new SceneDragHandler(contentBrowser);
            }

            sceneDragHandler.OnEnable();

        }

        public void OnDisable()
        {
            // scene drag handler
            sceneDragHandler.OnDisable();
            sceneDragHandler = null;
        }

        /// <summary>
        /// Get the width of the info / description panel depending on whether the description is visible or not
        /// </summary>
        /// <returns></returns>
        private int GetInfoWidth()
        {
            return contentBrowser.IsDescriptionVisible() ? infoWidth : 0;
        }

        public void Draw(List<PresetItem> presets, Grouping grouping)
        {
            int cellWidth = cellSize;
            int cellHeight = cellSize;
            float safetyMargin = cellWidth / 2f; // just some margin to keep the stamp preview almost fully visible
            float verticalScrollbarWidth = 22f;
            float gridWidth = EditorGUIUtility.currentViewWidth - contentBrowser.GetListWidth() - safetyMargin - GetInfoWidth() - verticalScrollbarWidth;
            int columnCount = Mathf.FloorToInt(gridWidth / cellWidth);

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical("Box");
                {
                    contentScrollPosition = EditorGUILayout.BeginScrollView(contentScrollPosition, GUILayout.Width(gridWidth + 30));
                    {
                        var groups = presets.GroupBy(x => x.collection.packName);

                        int groupIndex = -1;
                        foreach (var group in groups)
                        {
                            groupIndex++;

                            string groupHeader = group.Key;

                            List<PresetItem> presetGroup = new List<PresetItem>();
                            presetGroup.AddRange(group);

                            if (grouping == Grouping.ContentType)
                            {
                                if (groupIndex > 0)
                                {
                                    GUILayout.Space(10f);
                                }

                                EditorGUILayout.LabelField(groupHeader, EditorStyles.miniBoldLabel);

                                // separator
                                GUILayout.Box(GUIContent.none, GUIStyles.SeparatorStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                                GUILayout.Space(2);

                            }

                            int gridRows = Mathf.CeilToInt((float)presetGroup.Count / columnCount);

                            for (int row = 0; row < gridRows; row++)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    for (int column = 0; column < columnCount; column++)
                                    {
                                        // get preset; we fill up the row/column grid, so at some point the preset will be null
                                        int index = column + row * columnCount;
                                        PresetItem presetItem = index < presetGroup.Count ? presetGroup[index] : null;

                                        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));

                                        if (presetItem != null)
                                        {
                                            Texture image = presetItem.guiContent.image;
                                            string label = presetItem.guiContent.text;
                                            if (selectedPresetItem != null && presetItem.content == selectedPresetItem.content)
                                            {
                                                var outline = rect;
                                                outline.max += Vector2.one;
                                                outline.min -= Vector2.one;

                                                Texture2D texture = EditorGUIUtility.isProSkin ? Texture2D.whiteTexture : Texture2D.blackTexture;
                                                GUI.DrawTexture(outline, texture, ScaleMode.ScaleToFit, false);

                                            }

                                            // check for context click
                                            if (Event.current.type == EventType.ContextClick)
                                            {
                                                Vector2 position = Event.current.mousePosition;

                                                Rect popupRect = new Rect(position, Vector2.zero); // 2nd parameter is the offset from mouse position

                                                PopupWindow.Show(popupRect, new ContentItemPopup(contentBrowser, selectedPresetItem));

                                                Event.current.Use();
                                            }

                                            // texture
                                            GUI.DrawTexture(rect, image != null ? image : Texture2D.blackTexture, ScaleMode.ScaleToFit, false);

                                            // label
                                            rect.height = GUIUtil.SelectionElementLabelStyle.CalcHeight(new GUIContent(label), rect.width);

                                            GUI.DrawTexture(rect, GUIUtil.LabelBackgroundTexture, ScaleMode.StretchToFill);
                                            GUI.Box(rect, label, GUIUtil.SelectionElementLabelStyle);

                                            if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                                            {
                                                selectedPresetItem = presetItem;

                                                contentBrowser.Repaint();
                                            }

                                            if (Event.current.type == EventType.MouseDrag && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                                            {
                                                sceneDragHandler.OnDragStart(presetItem);

                                                Event.current.Use();
                                            }
                                        }
                                    }

                                    // stretch, so that the content doesn't move position (flicker) when we resize the editorwindow
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndVertical();

                if (contentBrowser.IsDescriptionVisible())
                {
                    GUILayout.BeginHorizontal("Box", GUILayout.Width(GetInfoWidth() - 20));
                    {
                        if (selectedPresetItem != null)
                        {
                            ContentData contentData = selectedPresetItem.content;
                            GUILayout.BeginVertical();
                            {
                                GUILayout.BeginVertical("box");
                                {
                                    EditorGUILayout.LabelField(selectedPresetItem.guiContent.text, EditorStyles.miniBoldLabel);

                                    if (!string.IsNullOrEmpty(contentData.description))
                                    {
                                        EditorGUILayout.LabelField(contentData.description, EditorStyles.wordWrappedMiniLabel);
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndVertical();

                                GUILayout.BeginVertical("box");
                                {
                                    EditorGUILayout.LabelField("Author", EditorStyles.miniBoldLabel);
                                    EditorGUILayout.LabelField(selectedPresetItem.collection.author, EditorStyles.miniLabel);
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndVertical();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();

            if (contentBrowser.IsHelpBoxVisible())
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Keyboard shortcuts", EditorStyles.miniBoldLabel);
                EditorGUILayout.Space(2);
                EditorGUILayout.LabelField("Shift at drag start: Set falloff override to global. For height stamps this scales the stamp to total terrain bounds.\nControl at drag start: For height stamps this positions the stamp at the center of the terrain.", EditorStyles.miniLabel);
                GUILayout.EndVertical();
            }
        }
    }
}