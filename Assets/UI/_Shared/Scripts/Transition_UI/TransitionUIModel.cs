using PhaseManager.Presenter;

namespace TransitionUI.Model {
    public class TransitionUIModel
    {
        public delegate void UpdateShowTransitionUIDel(bool isOpen);
        public event UpdateShowTransitionUIDel UpdateShowTransitionUI;
        public delegate void ShowRewardsUIUpdatedDel(bool isOpen);
        public event ShowRewardsUIUpdatedDel ShowRewardsUIUpdated;

        public delegate void UpdateTransitionTextDel(string transitionText);
        public event UpdateTransitionTextDel UpdateTransitionText;

        private bool IsTransitionUIOpen = false;
        private bool IsRewardsUIOpen = false;
        private string TransitionText;

        public TransitionUIModel()
        {
            PhasePresenter.Instance.OnUpdateIsRewardsUIOpen += SetIsRewardsUIOpen;
            PhasePresenter.Instance.OnUpdateTransitionUIText += SetTransitionText;
            PhasePresenter.Instance.OnUpdateShowTransitionUI += SetIsTransitionUIOpen;
        }

        private void SetIsTransitionUIOpen(bool isOpen)
        {
            IsTransitionUIOpen = isOpen;
            UpdateShowTransitionUI?.Invoke(isOpen);
        }

        public bool GetIsTransitionUIOpen()
        {
            return IsTransitionUIOpen;
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