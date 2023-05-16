namespace PhaseStateMachine
{
    public abstract class PhaseStateMachine
    {
        protected PhaseState State;

        public void SetState(PhaseState state)
        {
            State = state;
        }
    }
} 