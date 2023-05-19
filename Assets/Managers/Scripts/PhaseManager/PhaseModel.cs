using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using Gotchi.Network;
using Gotchi.Lickquidator.Manager;
using Fusion;

namespace PhaseManager
{    public enum Phase
    {
        None,
        Prep,
        Survival,
        Transitioning,
        Defeat,
        Victory,
    };
    namespace Model
    {
        public class PhaseModel: NetworkBehaviour
        {
            #region Fields
            [Header("Attributes")]
            [SerializeField] private GeneralSO generalSO = null;
            public GeneralSO GeneralSO { get { return generalSO; } }

            [SerializeField] private string prepPhaseText = "Prep Phase Starting...";
            public string PrepPhaseText { get { return prepPhaseText; } }

            [SerializeField] private string survivalPhaseText = "Survival Phase Starting...";
            public string SurvivalPhaseText { get { return survivalPhaseText; } }

            [SerializeField] private string defeatPhaseText = "Your Gotchi Has Died...";
            public string DefeatPhaseText { get { return defeatPhaseText; } }

            [SerializeField] private string victoryPhaseText = "You Won!";
            public string VictoryPhaseText { get { return victoryPhaseText; } }

            [SerializeField] private int numSecondsOnRewardsScreen = 8;
            public int NumSecondsOnRewardsScreen { get { return numSecondsOnRewardsScreen; } }

            [SerializeField] private int numSecondsOnNonRewardsScreen = 2;
            public int NumSecondsOnNonRewardsScreen { get { return numSecondsOnNonRewardsScreen; } }
            #endregion

            #region Events
            public event Action OnCurrentPhaseUpdated = delegate {};
            public event Action OnIsTransitioningUpdated = delegate {};
            public event Action OnCountdownValueUpdated = delegate {};
            #endregion

            #region Private Variables
            private Phase currentPhase = Phase.None;
            public Phase CurrentPhase { get { return currentPhase; } }
            private bool isTransitioning = false;
            public bool IsTransitioning { get { return isTransitioning; } }

            [Networked(OnChanged = nameof(OnSetCountdown))] 
            public float CountdownTracker { get; set; } = 0f;
            #endregion

            #region Unity Functions
            public override void FixedUpdateNetwork()
            {
                if (!Object.HasStateAuthority) return;

                if (isTransitioning || currentPhase == Phase.Transitioning || currentPhase == Phase.None) return;

                if (currentPhase == Phase.Prep)
                {
                    TrackPhaseCountdown();
                }
                else if (currentPhase == Phase.Survival)
                {
                    HandleEndSurvivalPhase();
                }
            }
            #endregion

            #region Public Functions
            public PhaseModel () {}

            public void SetCurrentPhase(Phase phase)
            {
                currentPhase = phase;

                OnCurrentPhaseUpdated();
            }


            public void SetIsTransitioning(bool isTransitioning)
            {
                this.isTransitioning = isTransitioning;

                OnIsTransitioningUpdated();
            }

            public void TrackPhaseCountdown()
            {
                float countdownTracker = Mathf.Clamp(CountdownTracker - Runner.DeltaTime, 0f, Mathf.Infinity);
                rpc_setCountdownTracker(countdownTracker);
            }

            public void StartCountdown()
            {
                CountdownTracker = generalSO.PrepPhaseCountdown;
            }

            public void HandleEndSurvivalPhase()
            {
                if (LickquidatorManager.Instance.ActiveLickquidators.Count == 0 && !NetworkManager.Instance.LocalPlayerGotchi.IsDead())
                {
                    rpc_startNextPhase();
                }
            }
            #endregion

            #region Network Functions
            [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
            private void rpc_startNextPhase()
            {
                print("Next Phase RPC called");
                SetIsTransitioning(true);
            }

            [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
            public void rpc_setCountdownTracker(float countdownTracker, RpcInfo info = default)
            {
                CountdownTracker = countdownTracker;
            }

            private void onCountdownUpdated()
            {
                OnCountdownValueUpdated();
                if (CountdownTracker <= 0f)
                {
                    rpc_startNextPhase();
                }
            }

            public static void OnSetCountdown(Changed<PhaseModel> changed)
            {
                changed.Behaviour.onCountdownUpdated();
            }
            #endregion
        }
    }
}

