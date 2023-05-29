using UnityEngine;
using System.Collections.Generic;

public class SplitterJumpManager : MonoBehaviour
{
    #region Public Variables
    public static SplitterJumpManager Instance = null;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject splitterJumpPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int splitterJumpPoolSize = 5;
    #endregion

    #region Private Variables
    private List<SplitterJump> splitterJumpPool = new List<SplitterJump>();
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

    void Start()
    {
        splitterJumpPool = CreateSplitterJumpPool(splitterJumpPrefab, splitterJumpPoolSize);
    }
    #endregion

    #region Public Functions
    public SplitterJump ActivateSplitterJump(Transform startTransform)
    {
        SplitterJump availableSplitterJump = GetAvailableSplitterJump();

        if (availableSplitterJump == null)
        {
            availableSplitterJump = CreateNewSplitterJump(splitterJumpPrefab);
            splitterJumpPool.Add(availableSplitterJump);
        }

        availableSplitterJump.transform.position = startTransform.position;
        availableSplitterJump.gameObject.SetActive(true);

        return availableSplitterJump;
    }
    #endregion

    #region Private Functions
    private SplitterJump CreateNewSplitterJump(GameObject splitterJumpPrefab)
    {
        GameObject splitterJumpObj = Instantiate(splitterJumpPrefab, Vector3.zero, Quaternion.identity, transform);
        SplitterJump splitterJump = splitterJumpObj.GetComponent<SplitterJump>();
        return splitterJump;
    }

    private SplitterJump GetAvailableSplitterJump()
    {
        foreach (SplitterJump splitterJump in splitterJumpPool)
        {
            if (!splitterJump.gameObject.activeInHierarchy)
            {
                return splitterJump;
            }
        }
        return null;
    }

    private List<SplitterJump> CreateSplitterJumpPool(GameObject splitterJumpPrefab, int maxPoolSize)
    {
        splitterJumpPrefab.SetActive(false);

        List<SplitterJump> splitterJumpPool = new List<SplitterJump>();
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject splitterJumpObj = Instantiate(splitterJumpPrefab, Vector3.zero, Quaternion.identity, transform);
            SplitterJump splitterJump = splitterJumpObj.GetComponent<SplitterJump>();
            splitterJumpPool.Add(splitterJump);
        }

        return splitterJumpPool;
    }
    #endregion
}