using UnityEngine;
using System.Collections.Generic;

public class CanMovePopUpManager : MonoBehaviour
{
    #region Public Variables
    public static CanMovePopUpManager Instance = null;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject canMovePopUpPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int canMovePopUpPoolSize = 50;
    [SerializeField] private Vector3 popUpHeightOffset = new Vector3(0, .25f, 0);
    #endregion

    #region Private Variables
    private List<CanMovePopUp> canMovePopUpPool = new List<CanMovePopUp>();
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
        canMovePopUpPool = CreateCanMovePopUpPool(canMovePopUpPrefab, canMovePopUpPoolSize);
    }
    #endregion

    #region Public Functions
    public void ShowCanMovePopUp(Vector3 position, bool canMoveToTarget)
    {
        foreach (CanMovePopUp popUp in canMovePopUpPool)
        {
            bool isPopUpNotAvailable = popUp.gameObject.activeInHierarchy;
            if (isPopUpNotAvailable) continue;

            popUp.transform.position = position + popUpHeightOffset;
            popUp.gameObject.SetActive(true);

            popUp.ShowCanMovePopUp(canMoveToTarget);
            return;
        }
    }
    #endregion

    #region Private Functions
    private List<CanMovePopUp> CreateCanMovePopUpPool(GameObject canMovePopUpPrefab, int maxPoolSize)
    {
        canMovePopUpPrefab.SetActive(false);

        List<CanMovePopUp> canMovePopUpPool = new List<CanMovePopUp>();
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject popUpObj = Instantiate(canMovePopUpPrefab, Vector3.zero, canMovePopUpPrefab.transform.rotation, transform);
            CanMovePopUp popUp = popUpObj.GetComponent<CanMovePopUp>();
            canMovePopUpPool.Add(popUp);
        }

        return canMovePopUpPool;
    }
    #endregion
}