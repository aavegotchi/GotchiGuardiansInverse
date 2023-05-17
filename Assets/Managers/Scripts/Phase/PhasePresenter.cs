using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gotchi.Events;
using Gotchi.Network;
using Fusion;
using PhaseManager.Model;

namespace PhaseManager {
    namespace Presenter {
        public class PhasePresenter: NetworkBehaviour
        {
            #region Public Variables
            public static PhasePresenter Instance = null;
            #endregion

            #region Fields
            [Header("Attributes")]
            [SerializeField] private GeneralSO generalSO = null;
            [SerializeField] public string prepPhaseText = "Prep Phase Starting...";
            [SerializeField] public string survivalPhaseText = "Survival Phase Starting...";
            [SerializeField] public string defeatPhaseText = "Your Gotchi Has Died...";
            [SerializeField] public string victoryPhaseText = "You Won!";
            [SerializeField] public int numSecondsOnRewardsScreen = 8;
            [SerializeField] public int numSecondsOnNonRewardsScreen = 2;
            #endregion

            #region Private Variables
            private PhaseModel Model;
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
            public delegate void OnUpdateTransitionUITextDel(string text);
            public event OnUpdateTransitionUITextDel OnUpdateTransitionUIText;
            public delegate void OnUpdateShowTransitionUIDel(bool isOpen);
            public event OnUpdateShowTransitionUIDel OnUpdateShowTransitionUI;

            #endregion

            #region Unity Functions
            void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    Model = new PhaseModel();
                    EventBus.GotchiEvents.GotchisAllDead += HandleDefeat;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            public override void FixedUpdateNetwork()
            {
                if (!Object.HasStateAuthority) return;

                if (Model.GetCurrentPhase() == Phase.Transitioning || Model.GetCurrentPhase() == Phase.None) return;

                if (Model.GetCurrentPhase() == Phase.Prep)
                {
                    TrackPhaseCountdown();
                }
                else if (Model.GetCurrentPhase() == Phase.Survival)
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

            public Phase GetCurrentPhase()
            {
                return Model.GetCurrentPhase();
            }
            #endregion

            #region Private Functions
            public void updatePhase(string nextPhaseStr)
            {
                Phase nextPhase = (Phase)Enum.Parse(typeof(Phase), nextPhaseStr);
                Model.SetCurrentPhase(nextPhase);

                if (Model.GetCurrentPhase() == Phase.Prep)
                {
                    CountdownTracker = generalSO.PrepPhaseCountdown;
                    OnUpdateShowCountdown?.Invoke(true);

                    EventBus.PhaseEvents.PrepPhaseStarted();
                }
                else if (Model.GetCurrentPhase() == Phase.Survival)
                {
                    OnUpdateShowCountdown?.Invoke(false);

                    EventBus.PhaseEvents.SurvivalPhaseStarted();
                }
                else if (Model.GetCurrentPhase() == Phase.Defeat)
                {
                    UserInterfaceManager.Instance.ShowGameOverUI();
                }
                else if (Model.GetCurrentPhase() == Phase.Victory)
                {
                    UserInterfaceManager.Instance.ShowGameOverUI();
                }

            }

            [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
            private void rpc_startNextPhase()
            {
                Phase nextPhase = Model.GetCurrentPhase() == Phase.Prep ? Phase.Survival : Phase.Prep;
                Model.SetCurrentPhase(Phase.Transitioning);

                EventBus.PhaseEvents.TransitionPhaseStarted(nextPhase);

                if (nextPhase == Phase.Prep)
                {
                    StatsManager.Instance.Money += StatsManager.Instance.GetEnemiesSpawnBonus();
                }

                StartCoroutine(showTransition(nextPhase));
            }

            private void HandleDefeat()
            {
                EventBus.PhaseEvents.TransitionPhaseStarted(Phase.Defeat);

                StartCoroutine(showTransition(Phase.Defeat));
            }

            private IEnumerator showTransition(Phase nextPhase)
            {
                OnUpdateShowTransitionUI?.Invoke(true);
                if (nextPhase == Phase.Prep)
                {
                    OnUpdateTransitionUIText?.Invoke(prepPhaseText);
                    OnUpdateIsRewardsUIOpen?.Invoke(true);

                    yield return new WaitForSeconds(numSecondsOnRewardsScreen);

                    OnUpdateIsRewardsUIOpen?.Invoke(false);

                    StatsManager.Instance.ClearCreateAndKillStats();
                }
                else if (nextPhase == Phase.Survival) {
                    OnUpdateTransitionUIText?.Invoke(survivalPhaseText);
                    yield return new WaitForSeconds(numSecondsOnNonRewardsScreen);
                } 
                else if (nextPhase == Phase.Defeat) {
                    OnUpdateTransitionUIText?.Invoke(defeatPhaseText);

                    yield return new WaitForSeconds(numSecondsOnNonRewardsScreen);
                } else if (nextPhase == Phase.Victory) {
                    OnUpdateTransitionUIText?.Invoke(victoryPhaseText);

                    yield return new WaitForSeconds(numSecondsOnNonRewardsScreen);
                }

                OnUpdateShowTransitionUI?.Invoke(false);

                updatePhase(nextPhase.ToString());
            }

            private void HandleEndSurvivalPhase()
            {
                if (EnemyPool.Instance.ActiveEnemies.Count == 0 && !NetworkManager.Instance.LocalPlayerGotchi.IsDead)
                {
                    rpc_startNextPhase();
                }
            }

            public void TrackPhaseCountdown()
            {
                float countdownTracker = Mathf.Clamp(CountdownTracker - Runner.DeltaTime, 0f, Mathf.Infinity);
                rpc_setCountdownTracker(countdownTracker);
            }

            #endregion

            #region Network Functions
            [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
            private void rpc_setCountdownTracker(float countdownTracker, RpcInfo info = default)
            {
                CountdownTracker = countdownTracker;
            }

            public static void OnSetCountdown(Changed<PhasePresenter> changed)
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
    }
}