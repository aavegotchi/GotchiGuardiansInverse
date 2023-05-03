using UnityEngine;
using System.Collections.Generic;

public class DamagePopUpManager : MonoBehaviour
{
    #region Public Variables
    public static DamagePopUpManager Instance = null;
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
        foreach (DamagePopUp_UI popUp in damagePopUpPool)
        {
            bool isPopUpNotAvailable = popUp.gameObject.activeInHierarchy;
            if (isPopUpNotAvailable) continue;

            popUp.SetFollowTransform(healthbarTransform);
            if(popUp.transform.position != this.transform.position) popUp.transform.position = healthbarTransform.position + damagePopUpOffset;
            popUp.gameObject.SetActive(true);

            popUp.ShowAndHide(damage, isEnemy);
            return;
        }
    }
    #endregion

    #region Private Functions
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