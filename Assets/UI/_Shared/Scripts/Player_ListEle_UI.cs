using Cinemachine;
using DG.Tweening;
using GameMaster;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_ListEle_UI : MonoBehaviour
{
    #region Public Variables
    public int PlayerId { get { return playerId; } }
    public CinemachineVirtualCamera VCamera { get { return vCamera; } }
    #endregion

    #region fields
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vCamera = null;

    [Header("Sub Objects")]
    [SerializeField] private TextMeshProUGUI playerNameLabel = null;
    [SerializeField] private Image bankImg = null;
    [SerializeField] private Image incomeImg = null;
    [SerializeField] private TextMeshProUGUI bankLabel = null;
    [SerializeField] private TextMeshProUGUI incomeLabel = null;
    [SerializeField] private TextMeshProUGUI hpLabel = null;
    [SerializeField] private RectTransform healthBarMask = null;

    [Header("Parent")]
    [SerializeField] private Players_List_UI parentList = null;

    [Header("Background")]
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Sprite defaultBackgroundSprite;
    [SerializeField] private Sprite deadBackgroundSprite;

    [Header("HP")]
    [SerializeField] private int maxHP = 250;
    [SerializeField] private float animTimePerHpAmount = 0.005f;
    [SerializeField] private int editorTargetHP;

    [Header("Monies")]
    [SerializeField] private int editorBankAmt = 100;
    [SerializeField] private int editorIncomeAmt = 100;
    [SerializeField] private Animator gainBankAnimator = null;
    [SerializeField] private Animator loseBankAnimator = null;
    [SerializeField] private Animator gainIncomeAnimator = null;
    [SerializeField] private Animator loseIncomeAnimator = null;
    #endregion

    #region private variables
    private int playerId = -1;

    // HP
    private int currentRenderedHP;
    private int lastAppliedTargetHp;
    private float fullHPMaskWidth;
    private Tween HPTweener;

    // Monies
    private int currentBankAmt;
    private int lastAppliedBankAmt;
    private int currentIncomeAmt;
    private int lastAppliedIncomeAmt;
    private Tween bankTweener;
    private Tween incomeTweener;
    #endregion

    #region public properties
    // #JS this is a bit of a hack until proper player data is plugged in
    public int CurrentHP
    {
        get { return lastAppliedTargetHp; }
    }
    #endregion

    #region Unity methods
    // Start is called before the first frame update
    void Start()
    {
        currentRenderedHP = maxHP;
        editorTargetHP = maxHP;
        lastAppliedTargetHp = maxHP;
        hpLabel.SetText(maxHP.ToString());

        currentBankAmt = editorBankAmt;
        lastAppliedBankAmt = editorBankAmt;
        bankLabel.SetText(currentBankAmt.ToString());

        currentIncomeAmt = editorIncomeAmt;
        lastAppliedIncomeAmt = editorIncomeAmt;
        incomeLabel.SetText(currentIncomeAmt.ToString());

        if (healthBarMask != null) {
            fullHPMaskWidth = healthBarMask.GetComponent<RectTransform>().sizeDelta.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lastAppliedBankAmt != editorBankAmt) {
            UpdateBank(editorBankAmt);
        }

        if (lastAppliedIncomeAmt != editorIncomeAmt) { 
            UpdateIncomeAmt(editorIncomeAmt);
        }
    }

    public void Click()
    {
        Debug.Log($"~~Click: {playerId}");
        List<Player_ListEle_UI> playerElements = UserInterfaceManager.Instance.PlayersListUI.playerElements;
        foreach(Player_ListEle_UI ele in playerElements)
        {
            if (ele.PlayerId == playerId) {
                ele.VCamera.gameObject.SetActive(true);
            } else {
                ele.VCamera.gameObject.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        GameMasterEvents.GotchiEvents.GotchiDamaged += HandleGotchiDamage;
    }

    void OnDisable() 
    {
        GameMasterEvents.GotchiEvents.GotchiDamaged -= HandleGotchiDamage;
    }
    #endregion

    #region public functions
    public void SetPlayerId(int id) 
    {
        playerId = id;
    }
    public void SetPlayerName(string playerName)
    {
        playerNameLabel.SetText(playerName);
    }

    public void UpdateHp(int targetHP)
    {
        if (lastAppliedTargetHp == targetHP) {
            return;
        }

        lastAppliedTargetHp = targetHP;
        editorTargetHP = targetHP;

        if (HPTweener != null) {
            HPTweener.Pause();
            HPTweener.Kill();
            HPTweener = null;
        }

        if (parentList != null) {
            parentList.CheckAndUpdatePlayerOrder();
        }

        float duration = animTimePerHpAmount * Mathf.Abs(lastAppliedTargetHp - currentRenderedHP);
        float ySize = healthBarMask.GetComponent<RectTransform>().sizeDelta.y;
        HPTweener = DOTween.To(() => currentRenderedHP, (x) => { 
            currentRenderedHP = x;  
            hpLabel.SetText(currentRenderedHP.ToString());
            healthBarMask.GetComponent<RectTransform>().sizeDelta = new Vector2(fullHPMaskWidth * currentRenderedHP / maxHP, ySize);
            }, lastAppliedTargetHp, duration);
    }

    public void UpdateBank(int bankAmt) {
        if (lastAppliedBankAmt == bankAmt) { 
            return;
        }

        if (bankAmt > lastAppliedBankAmt && gainBankAnimator != null) {
            gainBankAnimator.SetTrigger("FallingIn");
        }
        else if (bankAmt < lastAppliedBankAmt && loseBankAnimator != null) {
            loseBankAnimator.SetTrigger("FallingOut");
        }

        lastAppliedBankAmt = bankAmt;
        editorBankAmt = bankAmt;

        if (bankTweener != null)
        {
            bankTweener.Pause();
            bankTweener.Kill();
            bankTweener = null;
        }

        bankTweener = DOTween.To(() => currentBankAmt, (x) => { currentBankAmt = x; bankLabel.SetText(currentBankAmt.ToString()); }, lastAppliedBankAmt, 0.5f);
    }

    public void UpdateIncomeAmt(int incomeAmt) {
        if (lastAppliedIncomeAmt == incomeAmt) { 
            return;
        }

        if (incomeAmt > lastAppliedIncomeAmt && gainBankAnimator != null)
        {
            gainIncomeAnimator.SetTrigger("FallingIn");
        }
        else if (incomeAmt < lastAppliedIncomeAmt && loseBankAnimator != null)
        {
            loseIncomeAnimator.SetTrigger("FallingOut");
        }

        lastAppliedIncomeAmt = incomeAmt;
        editorIncomeAmt = incomeAmt;

        if (incomeTweener != null)
        {
            incomeTweener.Pause();
            incomeTweener.Kill();
            incomeTweener = null;
        }

        incomeTweener = DOTween.To(() => currentIncomeAmt, (x) => { currentIncomeAmt = x; incomeLabel.SetText(currentIncomeAmt.ToString()); }, lastAppliedIncomeAmt, 0.5f);
    }

    public void HandleGotchiDamage(int id, int damage)
    {
        if (id == playerId)
        {
            UpdateHp(lastAppliedTargetHp - damage);
        }
    }
    #endregion
}
