using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    #region Public Variables
    public static AbilityManager Instance = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Functions
    public void TriggerSpinAttack()
    {
        GotchiManager.Instance.Player.SpinAttack();
    }
    #endregion
}
