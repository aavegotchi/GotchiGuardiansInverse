using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameMaster;
using Gotchi.Network;
using Gotchi.Lickquidator.Manager;
using Fusion;

namespace PhaseManager
{    
    public enum Phase
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
            public event Action<Phase> OnCurrentPhaseUpdated = delegate {};
            public event Action<Phase> OnNextPhaseUpdated = delegate {};
            public event Action OnCountdownValueUpdated = delegate {};
            #endregion

            #region Public Variables
            public Phase CurrentPhase { get { return currentPhase; } }
            public Phase NextPhase { get { return nextPhase; } }
            #endregion

            #region Private Variables
            private Phase currentPhase = Phase.None;
            private Phase nextPhase = Phase.None;

            [Networked(OnChanged = nameof(OnSetCountdown))] 
            public float CountdownTracker { get; set; } = 0f;
            #endregion

            #region Unity Functions
            public override void FixedUpdateNetwork()
            {
                if (!Object.HasStateAuthority) return;

                if (nextPhase != Phase.None || currentPhase == Phase.Transitioning || currentPhase == Phase.None) return;

                if (currentPhase == Phase.Prep)
                {
                    TrackPhaseCountdown();
                }
                else if (currentPhase == Phase.Survival)
                {
                    HandleEndSurvivalPhase();
                }
            }

            void OnEnable() 
            {
                GameMasterEvents.GotchiEvents.GotchiDied += HandleGotchiDied;
            }

            void OnDisable() 
            {
                GameMasterEvents.GotchiEvents.GotchiDied -= HandleGotchiDied;
            }
            #endregion

            #region Public Functions
            public PhaseModel () {}

            public void SetCurrentPhase(Phase phase)
            {
                currentPhase = phase;

                OnCurrentPhaseUpdated(phase);
            }


            public void SetNextPhase(Phase nextPhase)
            {
                this.nextPhase = nextPhase;

                OnNextPhaseUpdated(nextPhase);
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
                    rpc_startNextPhase(Phase.Prep);
                }
            }
            #endregion

            #region Network Functions
            [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
            private void rpc_startNextPhase(Phase phase)
            {
                print("Next Phase RPC called");
                SetNextPhase(phase);
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
                    rpc_startNextPhase(Phase.Survival);
                }
            }

            public static void OnSetCountdown(Changed<PhaseModel> changed)
            {
                changed.Behaviour.onCountdownUpdated();
            }
            #endregion

            #region Private Functions
            public void HandleGotchiDied(int gotchiId)
            {
                if (gotchiId == NetworkManager.Instance.LocalPlayerGotchi.gameObject.GetInstanceID())
                {
                    SetNextPhase(Phase.Defeat);
                } else if (GotchiManager.Instance.GetLiveGotchiCount() == 1 && GotchiManager.Instance.GetLiveBotCount() == 0) {
                    SetNextPhase(Phase.Victory);
                }
            }
            #endregion
        }
    }
}

