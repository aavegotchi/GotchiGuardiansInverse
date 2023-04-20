using System.Collections.Generic;
using UnityEngine;

public class GameSettingsTab_UI : MonoBehaviour
{
    public ScriptableObject[] scriptableObjects;
    public GameObject gameSettingsElementPrefab;
    private Transform contentTransform;


    private void Awake()
    {
        contentTransform = transform.Find("Viewport/Content");
    }

    private void Start()
    {
        foreach (ScriptableObject scriptableObject in scriptableObjects)
        {
            InitializeScriptableObject(scriptableObject);
        }
    }

    private void InitializeScriptableObject(ScriptableObject scriptableObject)
    {
        System.Type scriptableObjectType = scriptableObject.GetType();
        var fields = scriptableObjectType.GetFields();
        int index = 0;

        foreach (var field in fields)
        {
            System.Type fieldType = field.FieldType;
            if (fieldType.IsSubclassOf(typeof(ScriptableObject)))
            {
                ScriptableObject childScriptableObject = (ScriptableObject)field.GetValue(scriptableObject);
                InitializeScriptableObject(childScriptableObject);
            }
            else
            {
                GameObject gameSettingsElementGO = Instantiate(gameSettingsElementPrefab, contentTransform);
                gameSettingsElementGO.name = scriptableObject + "-gameSettingsElement-" + index;
                GameSettingsElement_UI gameSettingsElementUI = gameSettingsElementGO.GetComponent<GameSettingsElement_UI>();
                gameSettingsElementUI.Initialize(field.Name, scriptableObject);
            }
            index++;
        }
    }
}
