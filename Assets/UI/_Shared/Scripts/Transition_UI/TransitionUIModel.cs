using System;
using PhaseManager.Presenter;

namespace TransitionUI {
    namespace Model {
        public class TransitionUIModel
        {
            public event Action<bool> OnShowTransitionUIUpdated = delegate {};
            public event Action<string> OnTransitionUITextUpdated = delegate {};
            public event Action<bool> OnIsRewardsUIOpenUpdated = delegate {};

            #region Private Variables
            public bool IsTransitionUIOpen { get { return isTransitionUIOpen; } }
            public bool IsRewardsUIOpen { get { return isRewardsUIOpen; } }
            public string TransitionText { get { return transitionText; } }
            #endregion

            #region Private Variables
            private bool isTransitionUIOpen = false;
            private bool isRewardsUIOpen = false;
            private string transitionText;
            #endregion


            public TransitionUIModel()
            {
                PhasePresenter.Instance.OnShowTransitionUIUpdated += SetIsTransitionUIOpen;
                PhasePresenter.Instance.OnTransitionUITextUpdated += SetTransitionText;
                PhasePresenter.Instance.OnIsRewardsUIOpenUpdated += SetIsRewardsUIOpen;
            }

            private void SetIsTransitionUIOpen(bool isOpen)
            {
                isTransitionUIOpen = isOpen;
                OnShowTransitionUIUpdated(isOpen);
            }

            private void SetTransitionText(string text)
            {
                transitionText = text;
                OnTransitionUITextUpdated(transitionText);
            }

            private void SetIsRewardsUIOpen(bool isOpen)
            {
                isRewardsUIOpen = isOpen;
                OnIsRewardsUIOpenUpdated(isRewardsUIOpen);
            }
        }
    }
}