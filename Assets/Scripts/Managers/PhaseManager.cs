using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gotchi.Events;

public class PhaseManager : MonoBehaviour
{
    #region Public Variables
    public static PhaseManager Instance = null;

    public enum Phase
    {
        None,
        Prep,
        Spawning,
        Survival,
        Transitioning
    };

    public Phase CurrentPhase { get; private set; } = Phase.None;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private Animator transitionScreenAnimator = null;
    [SerializeField] private TextMeshProUGUI transitionCountdownTextUI = null;
    [SerializeField] private CountdownTimer_UI countdownTimer = null;
    [SerializeField] private GameObject rewardsScreenUI = null;

    [SerializeField] private TextMeshProUGUI pawnLickquidatorRewardTextUI = null;
    [SerializeField] private TextMeshProUGUI aerialLickquidatorRewardTextUI = null;
    [SerializeField] private TextMeshProUGUI bossLickquidatorRewardTextUI = null;
    [SerializeField] private TextMeshProUGUI basicTowerRewardTextUI = null;
    [SerializeField] private TextMeshProUGUI arrowTowerRewardTextUI = null;
    [SerializeField] private TextMeshProUGUI fireTowerRewardTextUI = null;
    [SerializeField] private TextMeshProUGUI iceTowerRewardTextUI = null;

    [SerializeField] private TextMeshProUGUI pawnLickquidatorCostTextUI = null;
    [SerializeField] private TextMeshProUGUI aerialLickquidatorCostTextUI = null;
    [SerializeField] private TextMeshProUGUI bossLickquidatorCostTextUI = null;
    [SerializeField] private TextMeshProUGUI basicTowerCostTextUI = null;
    [SerializeField] private TextMeshProUGUI arrowTowerCostTextUI = null;
    [SerializeField] private TextMeshProUGUI fireTowerCostTextUI = null;
    [SerializeField] private TextMeshProUGUI iceTowerCostTextUI = null;

    [SerializeField] private TextMeshProUGUI enemiesSpawnBonusTextUI = null;

    [SerializeField] private TextMeshProUGUI netTextUI = null;

    [Header("Attributes")]
    [SerializeField] private GeneralSO generalSO = null;
    [SerializeField] private string prepPhaseText = "Prep Phase Starting...";
    [SerializeField] private string survivalPhaseText = "Survival Phase Starting...";
    [SerializeField] private int numSecondsOnRewardsScreen = 8;
    [SerializeField] private int numSecondsOnNonRewardsScreen = 2;
    #endregion

    #region Private Variables
    private float countdownTracker = 0f;
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

    void Update()
    {
        if (CurrentPhase == Phase.Transitioning || CurrentPhase == Phase.None) return;

        if (CurrentPhase == Phase.Prep)
        {
            TrackPhaseCountdown();
        }
        else if (CurrentPhase == Phase.Survival)
        {
            HandleEndSurvivalPhase();
        }
    }
    #endregion

    #region Private Functions
    public void StartNextPhase()
    {
        if (CurrentPhase == Phase.None)
        {
            updatePhase(Phase.Prep);
            return;
        }

        Phase nextPhase = CurrentPhase == Phase.Prep ? Phase.Survival : Phase.Prep;
        CurrentPhase = Phase.Transitioning;

        EventBus.PhaseEvents.TransitionPhaseStarted(nextPhase);

        if (nextPhase == Phase.Prep)
        {
            transitionCountdownTextUI.text = prepPhaseText;
            StatsManager.Instance.Money += StatsManager.Instance.GetEnemiesSpawnBonus();
        }
        else if (nextPhase == Phase.Survival)
        {
            transitionCountdownTextUI.text = survivalPhaseText;
        }

        StartCoroutine(showTransition(nextPhase));
    }

    private IEnumerator showTransition(Phase nextPhase)
    {
        transitionScreenAnimator.SetTrigger("Open");

        if (nextPhase == Phase.Prep)
        {
            rewardsScreenUI.SetActive(true);

            int pawnLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(EnemyManager.EnemyType.PawnLickquidator);
            int aerialLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(EnemyManager.EnemyType.AerialLickquidator);
            int bossLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(EnemyManager.EnemyType.BossLickquidator);

            // TODO: account for upgraded towers
            int basicTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerManager.TowerType.BasicTower);
            int arrowTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerManager.TowerType.ArrowTower1);
            int fireTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerManager.TowerType.FireTower1);
            int iceTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerManager.TowerType.IceTower1);

            int pawnLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(EnemyManager.EnemyType.PawnLickquidator);
            int aerialLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(EnemyManager.EnemyType.AerialLickquidator);
            int bossLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(EnemyManager.EnemyType.BossLickquidator);

            // TODO: account for upgraded towers
            int basicTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerManager.TowerType.BasicTower);
            int arrowTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerManager.TowerType.ArrowTower1);
            int fireTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerManager.TowerType.FireTower1);
            int iceTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerManager.TowerType.IceTower1);

            int enemiesSpawnReward = StatsManager.Instance.GetEnemiesSpawnBonus();

            pawnLickquidatorRewardTextUI.text = $"${pawnLickquidatorKillCosts}";
            aerialLickquidatorRewardTextUI.text = $"${aerialLickquidatorKillCosts}";
            bossLickquidatorRewardTextUI.text = $"${bossLickquidatorKillCosts}";

            basicTowerRewardTextUI.text = $"${basicTowerKillCosts}";
            arrowTowerRewardTextUI.text = $"${arrowTowerKillCosts}";
            fireTowerRewardTextUI.text = $"${fireTowerKillCosts}";
            iceTowerRewardTextUI.text = $"${iceTowerKillCosts}";

            pawnLickquidatorCostTextUI.text = $"${pawnLickquidatorCreateCosts}";
            aerialLickquidatorCostTextUI.text = $"${aerialLickquidatorCreateCosts}";
            bossLickquidatorCostTextUI.text = $"${bossLickquidatorCreateCosts}";

            basicTowerCostTextUI.text = $"${basicTowerCreateCosts}";
            arrowTowerCostTextUI.text = $"${arrowTowerCreateCosts}";
            fireTowerCostTextUI.text = $"${fireTowerCreateCosts}";
            iceTowerCostTextUI.text = $"${iceTowerCreateCosts}";

            enemiesSpawnBonusTextUI.text = $"${enemiesSpawnReward}";

            int net = pawnLickquidatorKillCosts + aerialLickquidatorKillCosts + bossLickquidatorKillCosts
                + basicTowerKillCosts + arrowTowerKillCosts + fireTowerKillCosts + iceTowerKillCosts
                - pawnLickquidatorCreateCosts - aerialLickquidatorCreateCosts - bossLickquidatorCreateCosts
                - basicTowerCreateCosts - arrowTowerCreateCosts - fireTowerCreateCosts - iceTowerCreateCosts
                + enemiesSpawnReward;

            netTextUI.text = $"{net}";

            yield return new WaitForSeconds(numSecondsOnRewardsScreen);

            StatsManager.Instance.ClearCreateAndKillStats();
        }
        else
        {
            rewardsScreenUI.SetActive(false);

            yield return new WaitForSeconds(numSecondsOnNonRewardsScreen);
        }

        transitionScreenAnimator.SetTrigger("Close");

        updatePhase(nextPhase);
    }

    private void HandleEndSurvivalPhase()
    {
        if (EnemyManager.Instance.ActiveEnemies.Count == 0 && !GotchiManager.Instance.Player.IsDead)
        {
           StartNextPhase();
        }
    }

    private void TrackPhaseCountdown()
    {
        if (countdownTracker <= 0f)
        {
            StartNextPhase();
        }

        countdownTracker -= Time.deltaTime;
        countdownTracker = Mathf.Clamp(countdownTracker, 0f, Mathf.Infinity);
        countdownTimer.SetTimeLeft(countdownTracker);
    }

    private void updatePhase(Phase nextPhase)
    {
        CurrentPhase = nextPhase;

        if (CurrentPhase == Phase.Prep)
        {
            countdownTracker = generalSO.PrepPhaseCountdown;
            countdownTimer.Show();

            EventBus.PhaseEvents.PrepPhaseStarted();
        }
        else if (CurrentPhase == Phase.Survival)
        {
            countdownTimer.Hide();

            EventBus.PhaseEvents.SurvivalPhaseStarted();
        }
    }
    #endregion
}