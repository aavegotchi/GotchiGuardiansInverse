using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gotchi.Network;

public class AbilityButton_UI : MonoBehaviour, IPointerClickHandler
{
    #region Public Variables
    public enum AbilityType
    {
        Immediate,
        Recast
    }
    #endregion

    #region Fields
    [SerializeField] private Sprite defaultSprite = null;
    [SerializeField] private Sprite highlightedSprite = null;
    [SerializeField] private float cooldownTime = 0f;
    [SerializeField] private AbilityType abilityType = AbilityType.Immediate;
    #endregion

    #region Private Variables
    private Image abilityImage = null;
    private Image cooldownImage = null;
    private bool isCooldown = false;
    private bool isHighlighted = false;
    #endregion

    #region Unity Functions
    void Awake()
    {
        abilityImage = GetComponent<Image>();
        cooldownImage = transform.Find("Cooldown").GetComponent<Image>();
    }

    void Start()
    {
        cooldownImage.fillAmount = 0f;
    }

    void Update()
    {
        HandleCooldown();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isHighlighted && !isCooldown)
        {
            ButtonPressed();
        }
        else
        {
            DehighlightButton();
        }
    }
    #endregion

    #region Public Functions
    public void AssignAbilityType(AbilityType nextType)
    {
        abilityType = nextType;
    }
    #endregion

    #region Private Functions
    private void HandleCooldown()
    {
        if (isCooldown)
        {
            cooldownImage.fillAmount -= 1 / cooldownTime * Time.deltaTime;
        }

        if (cooldownImage.fillAmount <= 0f)
        {
            cooldownImage.fillAmount = 0f;
            isCooldown = false;
        }
    }

    private void ButtonPressed()
    {
        HandleButtonActionBasedOnTypeAndState();
    }

    private void HandleButtonActionBasedOnTypeAndState() { 
        if (abilityType == AbilityType.Immediate)
        {
            AbilityTriggered();
        } 
        else if (abilityType == AbilityType.Recast && !isHighlighted)
        {
            isHighlighted = true;
            abilityImage.sprite = highlightedSprite;
        } 
        else if (abilityType == AbilityType.Recast && isHighlighted)
        {
            isHighlighted = false;
            AbilityTriggered();
        }
    }

    private void DehighlightButton()
    {
        isHighlighted = false;
        abilityImage.sprite = defaultSprite;
    }

    public void AbilityTriggered()
    {
        NetworkManager.Instance.LocalPlayerGotchi.SpinAttack();
        isCooldown = true;
        cooldownImage.fillAmount = 1;
    }
    #endregion
}