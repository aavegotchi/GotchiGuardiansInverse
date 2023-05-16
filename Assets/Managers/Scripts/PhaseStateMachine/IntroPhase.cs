namespace PhaseStateMachine
{
    public class IntroPhase: PhaseState
    {
        public IntroPhase(PhaseManager phaseManager): base(phaseManager) {}

        public void FixedUpdateNetwork()
        {
        }

        public void StartNextPhase() {
        }
    }
}