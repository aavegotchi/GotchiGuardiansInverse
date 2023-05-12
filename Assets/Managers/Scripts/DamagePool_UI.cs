using UnityEngine;
using System.Collections.Generic;

public class DamagePool_UI : MonoBehaviour
{
    #region Public Variables
    public static DamagePool_UI Instance = null;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject damagePopUpPrefab = null;
    [SerializeField] Vector3 damagePopUpOffset = new Vector3(0, 2, 2);

    [Header("Attributes")]
    [SerializeField] private int damagePopUpPoolSize = 50;
    #endregion

    #region Private Variables
    private List<DamagePopUp_UI> damagePopUpPool = new List<DamagePopUp_UI>();
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
        damagePopUpPool = CreateDamagePopUpPool(damagePopUpPrefab, damagePopUpPoolSize);
    }
    #endregion

    #region Public Functions
    public void ShowDamagePopUpAndColorDifferentlyIfEnemy(Transform healthbarTransform, float damage, bool isEnemy)
    {
        DamagePopUp_UI availablePopUp = GetAvailableDamagePopUp();

        if (availablePopUp == null)
        {
            availablePopUp = CreateNewDamagePopUp(damagePopUpPrefab);
            damagePopUpPool.Add(availablePopUp);
        }

        availablePopUp.SetFollowTransform(healthbarTransform);
        if (availablePopUp.transform.position != this.transform.position) availablePopUp.transform.position = healthbarTransform.position + damagePopUpOffset;
        availablePopUp.gameObject.SetActive(true);

        availablePopUp.ShowAndHide(damage, isEnemy);
    }
    #endregion

    #region Private Functions
    private DamagePopUp_UI CreateNewDamagePopUp(GameObject damagePopUpPrefab)
    {
        GameObject popUpObj = Instantiate(damagePopUpPrefab, Vector3.zero, Quaternion.identity, transform);
        DamagePopUp_UI popUp = popUpObj.GetComponent<DamagePopUp_UI>();
        popUp.SetDamagePopUpManager(this);
        return popUp;
    }

    private DamagePopUp_UI GetAvailableDamagePopUp()
    {
        foreach (DamagePopUp_UI popUp in damagePopUpPool)
        {
            if (!popUp.gameObject.activeInHierarchy)
            {
                return popUp;
            }
        }
        return null;
    }

    private List<DamagePopUp_UI> CreateDamagePopUpPool(GameObject damagePopUpPrefab, int maxPoolSize)
    {
        damagePopUpPrefab.SetActive(false);

        List<DamagePopUp_UI> damagePopUpPool = new List<DamagePopUp_UI>();
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject popUpObj = Instantiate(damagePopUpPrefab, Vector3.zero, Quaternion.identity, transform);
            DamagePopUp_UI popUp = popUpObj.GetComponent<DamagePopUp_UI>();
            popUp.SetDamagePopUpManager(this);
            damagePopUpPool.Add(popUp);
        }

        return damagePopUpPool;
    }
    #endregion
}