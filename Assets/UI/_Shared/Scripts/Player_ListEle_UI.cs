using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Fusion;
using System.Globalization;

public class Player_ListEle_UI : NetworkBehaviour
{
    #region fields
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
    [SerializeField] private int maxHP = 1000;
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

    #region Properties
    // #JS this is a bit of a hack until proper player data is plugged in
    public int CurrentHP
    {
        get { return lastAppliedTargetHp; }
    }
    #endregion

    #region Fusion RPCs
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_UpdateHP(int targetHP)
    {
        if (lastAppliedTargetHp == targetHP)
        {
            return;
        }

        lastAppliedTargetHp = targetHP;
        editorTargetHP = targetHP;

        if (HPTweener != null)
        {
            HPTweener.Pause();
            HPTweener.Kill();
            HPTweener = null;
        }

        if (parentList != null)
        {
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_UpdateBank(int bankAmt)
    {
        if (lastAppliedBankAmt == bankAmt)
        {
            return;
        }

        if (bankAmt > lastAppliedBankAmt && gainBankAnimator != null)
        {
            gainBankAnimator.SetTrigger("FallingIn");
        }
        else if (bankAmt < lastAppliedBankAmt && loseBankAnimator != null)
        {
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_UpdateIncomeAmt(int incomeAmt)
    {
        if (lastAppliedIncomeAmt == incomeAmt)
        {
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
    #endregion

    #region Unity methods
    // Start is called before the first frame update
    void Start()
    {
        currentRenderedHP = maxHP;
        editorTargetHP = maxHP;
        lastAppliedTargetHp = maxHP;
        hpLabel.SetText(currentRenderedHP.ToString());

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
        if (lastAppliedTargetHp != editorTargetHP) {
            UpdateHp(editorTargetHP);
        }

        if (lastAppliedBankAmt != editorBankAmt) {
            UpdateBank(editorBankAmt);
        }

        if (lastAppliedIncomeAmt != editorIncomeAmt) { 
            UpdateIncomeAmt(editorIncomeAmt);
        }
    }
    #endregion

    #region public functions
    public void SetPlayerName(string playerName)
    {
        playerNameLabel.SetText(playerName);
    }

    public void UpdateHp(int targetHP)
    {
        RPC_UpdateHP(targetHP);
    }

    public void UpdateBank(int bankAmt) {
        RPC_UpdateBank(bankAmt);
    }

    public void UpdateIncomeAmt(int incomeAmt) {
        RPC_UpdateBank(incomeAmt);
    }
    #endregion
}
