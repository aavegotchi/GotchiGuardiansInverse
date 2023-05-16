namespace TransitionUI.Model {
    public class TransitionUIModel
    {
        public delegate void ShowRewardsUIUpdatedDel(bool isOpen);
        public event ShowRewardsUIUpdatedDel ShowRewardsUIUpdated;

        private bool IsRewardsUIOpen = false;

        public TransitionUIModel()
        {
            PhaseManager.Instance.OnUpdateIsRewardsUIOpen += SetIsRewardsUIOpen;
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
    }
}