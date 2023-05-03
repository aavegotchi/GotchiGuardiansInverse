using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsElement_UI : MonoBehaviour
{
    public Color32 headerColor = new Color32(0, 120, 215, 255);  // A shade of blue
    public TextMeshProUGUI FieldNameText;
    public GameObject InputFieldContainer;

    public GameObject inputPrefab;
    public GameObject togglePrefab;
    public GameObject dropdownPrefab;

    private string fieldName;
    private ScriptableObject scriptableObject;

    public void Initialize(string fieldName, ScriptableObject scriptableObject)
    {
        this.fieldName = fieldName;
        this.scriptableObject = scriptableObject;

        FieldNameText.text = AddSpacesToFieldName(fieldName);

        System.Type fieldType = scriptableObject.GetType().GetField(fieldName).FieldType;

        if (fieldType == typeof(string))
        {
            FieldNameText.text = (string)scriptableObject.GetType().GetField(fieldName).GetValue(scriptableObject);
            FieldNameText.color = headerColor;
            FieldNameText.fontStyle = FontStyles.Bold;
            FieldNameText.fontSize = 24;
        }

        else if (fieldType == typeof(int) || fieldType == typeof(float))
        {
            GameObject inputFieldGO = Instantiate(inputPrefab, InputFieldContainer.transform);
            TMP_InputField inputField = inputFieldGO.GetComponent<TMP_InputField>();
            inputField.contentType = fieldType == typeof(int) ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;
            inputField.text = scriptableObject.GetType().GetField(fieldName).GetValue(scriptableObject).ToString();
            inputField.onEndEdit.AddListener(UpdateValue);
        }
        else if (fieldType == typeof(bool))
        {
            GameObject toggleGO = Instantiate(togglePrefab, InputFieldContainer.transform);
            Toggle toggle = toggleGO.GetComponent<Toggle>();
            toggle.isOn = (bool)scriptableObject.GetType().GetField(fieldName).GetValue(scriptableObject);
            toggle.onValueChanged.AddListener(UpdateToggleValue);
        }
        else if (fieldType.IsEnum)
        {
            GameObject dropdownGO = Instantiate(dropdownPrefab, InputFieldContainer.transform);
            TMP_Dropdown dropdown = dropdownGO.GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();

            string[] enumNames = Enum.GetNames(fieldType);
            List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
            foreach (string enumName in enumNames)
            {
                dropdownOptions.Add(new TMP_Dropdown.OptionData(enumName));
            }
            dropdown.AddOptions(dropdownOptions);

            Enum currentValue = (Enum)scriptableObject.GetType().GetField(fieldName).GetValue(scriptableObject);
            dropdown.value = Array.IndexOf(enumNames, currentValue.ToString());
            dropdown.onValueChanged.AddListener(UpdateDropdownValue);
        }

        else
        {
            Debug.Log("Unknown type");
        }
    }

    private void UpdateValue(string newValue)
    {
        System.Type fieldType = scriptableObject.GetType().GetField(fieldName).FieldType;
        if (fieldType == typeof(int))
        {
            scriptableObject.GetType().GetField(fieldName).SetValue(scriptableObject, int.Parse(newValue));
        }
        else if (fieldType == typeof(float))
        {
            scriptableObject.GetType().GetField(fieldName).SetValue(scriptableObject, float.Parse(newValue));
        }
    }

    private void UpdateToggleValue(bool newValue)
    {
        scriptableObject.GetType().GetField(fieldName).SetValue(scriptableObject, newValue);
    }

    private void UpdateDropdownValue(int newIndex)
    {
        System.Type fieldType = scriptableObject.GetType().GetField(fieldName).FieldType;
        if (fieldType.IsEnum)
        {
            scriptableObject.GetType().GetField(fieldName).SetValue(scriptableObject, Enum.GetValues(fieldType).GetValue(newIndex));
        }
    }

    private string AddSpacesToFieldName(string originalName)
    {
        if (string.IsNullOrEmpty(originalName))
            return string.Empty;

        string newName = originalName[0].ToString();
        for (int i = 1; i < originalName.Length; i++)
        {
            if (char.IsUpper(originalName[i]) && !char.IsWhiteSpace(originalName[i - 1]))
                newName += " ";

            newName += originalName[i];
        }
        return newName;
    }
}