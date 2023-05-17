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
        public class PhaseModel
        {
            #region Events
            public delegate void UpdateCurrentPhaseDel(Phase phase);
            public event UpdateCurrentPhaseDel UpdateCurrentPhase;

            public delegate void UpdateIsTransitioningDel(bool IsTransitioning);
            public event UpdateIsTransitioningDel UpdateIsTransitioning;
            #endregion

            #region Private Variables
            private Phase CurrentPhase = Phase.None;

            private bool IsTransitioning = false;
            #endregion

            #region Public Functions
            public PhaseModel () {}

            public void SetCurrentPhase(Phase phase)
            {
                CurrentPhase = phase;

                UpdateCurrentPhase?.Invoke(phase);
            }

            public Phase GetCurrentPhase()
            {
                return CurrentPhase;
            }

            public void SetIsTransitioning(bool isTransitioning)
            {
                IsTransitioning = isTransitioning;

                UpdateIsTransitioning?.Invoke(isTransitioning);
            }

            public bool GetIsTransitioning()
            {
                return IsTransitioning;
            }

            #endregion
        }
    }
}

