namespace PhaseStateMachine
{
    public class DefeatPhase: PhaseState
    {
        public DefeatPhase(PhaseManager phaseManager): base(phaseManager) {}

        public void FixedUpdateNetwork()
        {
        }

        public void StartNextPhase() {
        }
    }
}