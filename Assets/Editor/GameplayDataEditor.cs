using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using System.Linq;

public class GameplayDataEditor : EditorWindow
{
    private GameplayData gameplayData;
    private Vector2 towerListScrollPosition;
    private int selectedTowerIndex = -1;
   // private TowerCoreData selectedTower;
    private Vector2 rightPaneScrollInspector;
    private bool isLoaded = false;
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
        LoadData();
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

        if (isLoaded && GUILayout.Button("Save"))
        {
            SaveData();
        }

        EditorGUILayout.EndHorizontal();

        if (!isLoaded)
        {
            EditorGUILayout.HelpBox("No GameplayData found. Press Load to create a new file with default values.", MessageType.Info);

            return;
        }

        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.TextField("Version", gameplayData.version);
        gameplayData.startingGold = EditorGUILayout.IntField("Starting Gold", gameplayData.startingGold);
        gameplayData.startingIncome = EditorGUILayout.IntField("Starting Income", gameplayData.startingIncome);
        

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

            currentEditingTemplate.DrawDataInspectors();

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
        gameplayData = GameplayData.LoadData();
        currentEditingTemplate = null;
        isLoaded = true;
    }

    private void SaveData()
    {
        gameplayData.SaveData();
    }

    #region Tower Data

    private void DrawTowerTemplateData()
    {
        EditorGUILayout.LabelField("Tower Templates", EditorStyles.boldLabel);
        towerListScrollPosition = EditorGUILayout.BeginScrollView(towerListScrollPosition, GUILayout.ExpandHeight(true));

        if (gameplayData != null && gameplayData.towerTemplates != null)
        {
            for (int i = 0; i < gameplayData.towerTemplates.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (gameplayData.towerTemplates[i] == currentEditingTemplate)
                {
                    GUI.color = Color.yellow; // Set the highlight color
                }
                EditorGUILayout.LabelField(gameplayData.towerTemplates[i].name);

                if (GUILayout.Button("Edit"))
                {
                    currentEditingTemplate = gameplayData.towerTemplates[i];
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