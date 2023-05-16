namespace TransitionUI.Model {
    public class TransitionUIModel
    {
        public delegate void ShowRewardsUIUpdatedDel(bool isOpen);
        public event ShowRewardsUIUpdatedDel ShowRewardsUIUpdated;

        public delegate void UpdateTransitionTextDel(string transitionText);
        public event UpdateTransitionTextDel UpdateTransitionText;

        private bool IsRewardsUIOpen = false;
        private string TransitionText;

        public TransitionUIModel()
        {
            PhaseManager.Instance.OnUpdateIsRewardsUIOpen += SetIsRewardsUIOpen;
            PhaseManager.Instance.OnUpdateTransitionUIText += SetTransitionText;
        }

        private void SetIsRewardsUIOpen(bool isRewardsUIOpen)
        {
            IsRewardsUIOpen = isRewardsUIOpen;
            ShowRewardsUIUpdated?.Invoke(isRewardsUIOpen);
        }

        public bool GetIsRewardsUIOpen()
        {
            return IsRewardsUIOpen;
        }

        private void SetTransitionText(string transitionText)
        {
            TransitionText = transitionText;
            UpdateTransitionText?.Invoke(transitionText);
        }

        public string GetTransitionText()
        {
            return TransitionText;
        }
    }
}