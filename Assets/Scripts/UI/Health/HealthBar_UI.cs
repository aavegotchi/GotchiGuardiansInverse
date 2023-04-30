using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    #region Public Variables
    public float MaxHealth
    {
        set { maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            takeDamageTimer = 0f;
            currentHealth = value;
        }
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private Image front = null;
    [SerializeField] private Image back = null;
    [SerializeField] private Image background = null;

    [Header("Attributes")]
    [SerializeField] private float maxHealth = 0f;
    [SerializeField] private float currentHealth = 0f;
    [SerializeField] private float takeDamageTimerMax = 2f;
    [SerializeField] private Color takeDamageColor = Color.red;
    [SerializeField] private Color frontColor = Color.white;
    [SerializeField] private Color backColor = Color.black;
    [SerializeField] private Color backgroundColor = Color.gray;
    #endregion

    #region Private Variables
    private float takeDamageTimer = 0f;
    #endregion

    #region Unity Functions
    void OnEnable()
    {
        gameObject.transform.localPosition = Vector3.zero;
    }

    void Start()
    {
        front.color = frontColor;
        back.color = backColor;
        background.color = backgroundColor;
    }

    void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        updateHealth();
        hideHealthbarIfFullorEmpty();
    }
    #endregion

    #region Public Functions
    public void SetHealthbarMaxHealth(float maxHealth)
    {
        CurrentHealth = maxHealth;
        MaxHealth = maxHealth;
    }

    public void ShowDamagePopUpAndColorDifferentlyIfEnemy(float damage, bool isEnemy)
    {
        DamagePopUpManager.Instance.ShowDamagePopUpAndColorDifferentlyIfEnemy(transform, damage, isEnemy);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(HealthBarManager.Instance.gameObject.transform, true);
        front.fillAmount = 1f;
        back.fillAmount = 1f;
    }
    #endregion

    #region Private Functions
    private void updateHealth()
    {
        float healthFraction = currentHealth / maxHealth;
        front.fillAmount = healthFraction;
        takeDamageTimer += Time.deltaTime;
        float percentComplete = takeDamageTimer / takeDamageTimerMax;
        back.fillAmount = Mathf.Lerp(back.fillAmount, healthFraction, percentComplete);

        if (percentComplete < 1f)
        {
            back.color = takeDamageColor;
        }
        else
        {
            back.color = backColor;
        }
    }

    private void hideHealthbarIfFullorEmpty()
    {
        if (currentHealth >= maxHealth)
        {
            front.gameObject.SetActive(false);
            back.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
        else
        {
            front.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
        }
    }
    #endregion
}
