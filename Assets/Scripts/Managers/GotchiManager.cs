using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotchiManager : MonoBehaviour
{
    #region Public Variables
    public static GotchiManager Instance = null;

    public enum AttackType
    {
        Basic,
        Spin,
    }

    public Player_Gotchi Player
    {
        set { playerGotchi = value; }
        get { return playerGotchi; }
    }

    public Transform Spawn
    {
        get { return spawn; }
    }

    public GameObject GotchiPrefab
    {
        get { return gotchiPrefab; }
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject gotchiPrefab = null;

    [Header("Attributes")]
    [SerializeField] private Transform spawn = null;
    #endregion

    #region Private Variables
    private Player_Gotchi playerGotchi = null;
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
}
