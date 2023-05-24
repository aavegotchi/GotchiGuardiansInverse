using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameMaster;
using Gotchi.Network;
using Gotchi.Lickquidator.Manager;
using PhaseManager.Model;

namespace PhaseManager {
    namespace Presenter {
        public class PhasePresenter: MonoBehaviour
        {
            public PhaseModel Model { get { return model; } }

            [Header("Model")]
            [SerializeField] private PhaseModel model = null;
            
            #region Public Variables
            public static PhasePresenter Instance = null;
            #endregion

            #region Events
            public event Action<bool> OnShowTransitionUIUpdated = delegate {};
            public event Action<string> OnTransitionUITextUpdated = delegate {};
            public event Action<bool> OnIsRewardsUIOpenUpdated = delegate {};
            public event Action<bool> OnShowCoundownUpdated = delegate {};
            public event Action<float> OnCountdownValueUpdated = delegate {};
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

            void OnEnable() 
            {
                model.OnCountdownValueUpdated += onCountdownUpdated;
                model.OnNextPhaseUpdated += HandlePhaseTransition;
                model.OnCurrentPhaseUpdated += HandleCurrentPhaseUpdated;
            }

            void OnDisable() 
            {
                model.OnCountdownValueUpdated -= onCountdownUpdated;
                model.OnNextPhaseUpdated -= HandlePhaseTransition;
                model.OnCurrentPhaseUpdated -= HandleCurrentPhaseUpdated;
            }
            #endregion

            #region Public Functions
            public void StartFirstPrepPhase()
            {
                model.SetNextPhase(Phase.Prep);
            }

            public Phase GetCurrentPhase()
            {
                return model.CurrentPhase;
            }
            #endregion

            #region Private Functions
            public void updatePhase(string nextPhaseStr)
            {
                Phase nextPhase = (Phase)Enum.Parse(typeof(Phase), nextPhaseStr);
                model.SetNextPhase(Phase.None);
                model.SetCurrentPhase(nextPhase);

                if (model.CurrentPhase == Phase.Prep)
                {
                    model.StartCountdown();
                    OnShowCoundownUpdated(true);

                    GameMasterEvents.PhaseEvents.PrepPhaseStarted();
                }
                else if (model.CurrentPhase == Phase.Survival)
                {
                    OnShowCoundownUpdated(false);

                    GameMasterEvents.PhaseEvents.SurvivalPhaseStarted();
                }
                else if (model.CurrentPhase == Phase.Defeat)
                {
                    UserInterfaceManager.Instance.ShowGameOverUI();
                }
                else if (model.CurrentPhase == Phase.Victory)
                {
                    UserInterfaceManager.Instance.ShowGameOverUI();
                }

            }

            private void HandlePhaseTransition(Phase nextPhase)
            {
                if (nextPhase != Phase.None) {
                    Phase prevPhase = model.CurrentPhase;
                    model.SetCurrentPhase(Phase.Transitioning);

                    GameMasterEvents.PhaseEvents.TransitionPhaseStarted(nextPhase);

                    if (nextPhase == Phase.Prep)
                    {
                        StatsManager.Instance.Money += StatsManager.Instance.GetEnemiesSpawnBonus();
                    }

                    StartCoroutine(showTransition(nextPhase, prevPhase));
                }
            }

            private void HandleCurrentPhaseUpdated(Phase phase)
            {
                GameMasterEvents.PhaseEvents.PhaseChanged(phase);
            }

            private void HandleDefeat()
            {
                GameMasterEvents.PhaseEvents.TransitionPhaseStarted(Phase.Defeat);

                StartCoroutine(showTransition(Phase.Defeat));
            }

            private IEnumerator showTransition(Phase nextPhase, Phase prevPhase = Phase.None)
            {
                OnShowTransitionUIUpdated(true);
                if (nextPhase == Phase.Prep)
                {
                    OnTransitionUITextUpdated(model.PrepPhaseText);
                    if (!(prevPhase == Phase.None)) {
                        OnIsRewardsUIOpenUpdated(true);
                         yield return new WaitForSeconds(model.NumSecondsOnRewardsScreen);
                        OnIsRewardsUIOpenUpdated(false);
                    } else {
                        yield return new WaitForSeconds(model.NumSecondsOnNonRewardsScreen);
                    }
                    
                   

                    StatsManager.Instance.ClearCreateAndKillStats();
                }
                else if (nextPhase == Phase.Survival) {
                    OnTransitionUITextUpdated(model.SurvivalPhaseText);
                    yield return new WaitForSeconds(model.NumSecondsOnNonRewardsScreen);
                } 
                else if (nextPhase == Phase.Defeat) {
                    OnTransitionUITextUpdated(model.DefeatPhaseText);

                    yield return new WaitForSeconds(model.NumSecondsOnNonRewardsScreen);
                } else if (nextPhase == Phase.Victory) {
                    OnTransitionUITextUpdated(model.VictoryPhaseText);

                    yield return new WaitForSeconds(model.NumSecondsOnNonRewardsScreen);
                }

                OnShowTransitionUIUpdated(false);

                updatePhase(nextPhase.ToString());
            }

            private void onCountdownUpdated()
            {
                OnCountdownValueUpdated(model.CountdownTracker);
            }
            #endregion
        }
    }
}