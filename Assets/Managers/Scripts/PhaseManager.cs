using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gotchi.Events;
using Gotchi.Network;
using Fusion;

public class PhaseManager : NetworkBehaviour
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
    [SerializeField] private TextMeshProUGUI transitionCountdownTextUI = null;

    [Header("Attributes")]
    [SerializeField] private GeneralSO generalSO = null;
    [SerializeField] private string prepPhaseText = "Prep Phase Starting...";
    [SerializeField] private string survivalPhaseText = "Survival Phase Starting...";
    [SerializeField] private int numSecondsOnRewardsScreen = 8;
    [SerializeField] private int numSecondsOnNonRewardsScreen = 2;
    #endregion

    #region Private Variables
    [Networked(OnChanged = nameof(OnSetCountdown))] 
    public float CountdownTracker { get; set; } = 0f;
    #endregion

    #region Events
    public delegate void OnUpdateIsRewardsUIOpenDel(bool isOpen);
    public event OnUpdateIsRewardsUIOpenDel OnUpdateIsRewardsUIOpen;
    public delegate void OnUpdateShowCountdownDel(bool isOpen);
    public event OnUpdateShowCountdownDel OnUpdateShowCountdown;

    public delegate void OnUpdateCountdownValueDel(float countdownTime);
    public event OnUpdateCountdownValueDel OnUpdateCountdownValue;

    
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

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

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

    #region Public Functions
    public void StartFirstPrepPhase()
    {
        updatePhase(Phase.Prep.ToString());
    }
    #endregion

    #region Private Functions
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void rpc_startNextPhase()
    {
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
        if (nextPhase == Phase.Prep)
        {
            OnUpdateIsRewardsUIOpen?.Invoke(true);

            yield return new WaitForSeconds(numSecondsOnRewardsScreen);

            OnUpdateIsRewardsUIOpen?.Invoke(false);

        }
        else
        {
            OnUpdateIsRewardsUIOpen?.Invoke(false);

            yield return new WaitForSeconds(numSecondsOnNonRewardsScreen);
        }

        updatePhase(nextPhase.ToString());
    }

    private void HandleEndSurvivalPhase()
    {
        if (EnemyPool.Instance.ActiveEnemies.Count == 0 && !NetworkManager.Instance.LocalPlayerGotchi.IsDead)
        {
           rpc_startNextPhase();
        }
    }

    private void TrackPhaseCountdown()
    {
        float countdownTracker = Mathf.Clamp(CountdownTracker - Runner.DeltaTime, 0f, Mathf.Infinity);
        rpc_setCountdownTracker(countdownTracker);
    }

    private void updatePhase(string nextPhaseStr)
    {
        Phase nextPhase = (Phase)Enum.Parse(typeof(Phase), nextPhaseStr);
        CurrentPhase = nextPhase;

        if (CurrentPhase == Phase.Prep)
        {
            CountdownTracker = generalSO.PrepPhaseCountdown;
            OnUpdateShowCountdown?.Invoke(true);

            EventBus.PhaseEvents.PrepPhaseStarted();
        }
        else if (CurrentPhase == Phase.Survival)
        {
            OnUpdateShowCountdown?.Invoke(false);

            EventBus.PhaseEvents.SurvivalPhaseStarted();
        }
    }
    #endregion

    #region Network Functions
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void rpc_setCountdownTracker(float countdownTracker, RpcInfo info = default)
    {
        CountdownTracker = countdownTracker;
    }

    public static void OnSetCountdown(Changed<PhaseManager> changed)
    {
        changed.Behaviour.onSetCountdown();
    }

    private void onSetCountdown()
    {
        OnUpdateCountdownValue?.Invoke(CountdownTracker);

        if (CountdownTracker <= 0f)
        {
            rpc_startNextPhase();
        }
    }
    #endregion
}