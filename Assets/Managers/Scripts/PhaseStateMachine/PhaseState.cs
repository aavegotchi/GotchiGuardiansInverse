namespace PhaseStateMachine
{
    public abstract class PhaseState
    {
        protected PhaseManager PhaseManager;

        public PhaseState(PhaseManager phaseManager)
        {
            PhaseManager = phaseManager;
        }

        public void FixedUpdateNetwork() {}

        public void StartNextPhase() {}

        public void UpdatePhase() {}
    }
}