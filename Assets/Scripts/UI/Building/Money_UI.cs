using UnityEngine;
using TMPro;

public class Money_UI : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private TextMeshProUGUI moneyText = null;
    #endregion

    #region Unity Functions
    void LateUpdate()
    {
        moneyText.text = $"{StatsManager.Instance.Money}";
    }
    #endregion
}