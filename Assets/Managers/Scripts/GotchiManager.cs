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

    public List<Player_Gotchi> Players
    {
        get { return playerGotchis; }
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
    private List<Player_Gotchi> playerGotchis = new List<Player_Gotchi>();
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
