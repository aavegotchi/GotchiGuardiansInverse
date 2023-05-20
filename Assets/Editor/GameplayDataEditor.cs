using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using System.Linq;

public class GameplayDataEditor : EditorWindow
{
    private Vector2 towerListScrollPosition;
    private int selectedTowerIndex = -1;
   // private TowerCoreData selectedTower;
    private Vector2 rightPaneScrollInspector;
    private string newTowerName = "New Tower";

    private float splitterWidth = 5f;
    private float leftPaneWidth = 300f;

    private IDataTemplate currentEditingTemplate = null;

    [MenuItem("Tools/Gameplay/GameplayDataEditor")]
    private static void OpenEditor()
    {
        GameplayDataEditor window = GetWindow<GameplayDataEditor>();
        window.titleContent = new GUIContent("GameplayData Editor");
        window.Show();
    }

    private void OnEnable()
    {
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // Left Pane: General Settings and Tower Templates List
        DrawLeftPane();

        // Splitter
        DrawSplitter();

        // Right Pane: Selected Tower Template Inspector
        DrawRightPane();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawLeftPane()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(leftPaneWidth));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            LoadData();
        }

        if (GUILayout.Button("Save"))
        {
            SaveData();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.TextField("Version", GameplayData.Singleton.version);
        GameplayData.Singleton.startingGold = EditorGUILayout.IntField("Starting Gold", GameplayData.Singleton.startingGold);
        GameplayData.Singleton.startingIncome = EditorGUILayout.IntField("Starting Income", GameplayData.Singleton.startingIncome);
        

        EditorGUILayout.Space();

        DrawTowerTemplateData();

        EditorGUILayout.EndVertical();
    }

    private void DrawSplitter()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(splitterWidth));
        GUILayout.Box("", GUILayout.Width(splitterWidth), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndVertical();

        Rect splitterRect = GUILayoutUtility.GetLastRect();

        // Handle the splitter drag
        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.ResizeHorizontal);
        if (Event.current.type == EventType.MouseDown && splitterRect.Contains(Event.current.mousePosition))
        {
            Event.current.Use();
        }
        
        if (Event.current.type == EventType.MouseDrag)
        {
            leftPaneWidth += Event.current.delta.x;
            leftPaneWidth = Mathf.Clamp(leftPaneWidth, 100f, position.width - 100f);
            Repaint();
        }
    }


    private void DrawRightPane()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));

        if (currentEditingTemplate != null)
        {
            EditorGUILayout.LabelField("Current Template Data", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            rightPaneScrollInspector = EditorGUILayout.BeginScrollView(rightPaneScrollInspector, GUILayout.ExpandHeight(true));

            currentEditingTemplate = currentEditingTemplate.DrawDataInspectors(GameplayData.Singleton);

            EditorGUILayout.EndScrollView();
            
        }
        else
        {
            EditorGUILayout.HelpBox("No Selected Data", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }

    private void LoadData()
    {
        GameplayData.ReloadSingleton();
        currentEditingTemplate = null;
    }

    private void SaveData()
    {
        GameplayData.Singleton.SaveData();
    }

    #region Tower Data

    private void DrawTowerTemplateData()
    {
        EditorGUILayout.LabelField("Tower Templates", EditorStyles.boldLabel);
        towerListScrollPosition = EditorGUILayout.BeginScrollView(towerListScrollPosition, GUILayout.ExpandHeight(true));

        if (GameplayData.Singleton != null && GameplayData.Singleton.towerTemplates != null)
        {
            for (int i = 0; i < GameplayData.Singleton.towerTemplates.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (GameplayData.Singleton.towerTemplates[i] == currentEditingTemplate)
                {
                    GUI.color = Color.yellow; // Set the highlight color
                }
                EditorGUILayout.LabelField(GameplayData.Singleton.towerTemplates[i].name);

                if (GUILayout.Button("Edit"))
                {
                    currentEditingTemplate = GameplayData.Singleton.towerTemplates[i];
                }
                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndScrollView();
    }

    #endregion
}