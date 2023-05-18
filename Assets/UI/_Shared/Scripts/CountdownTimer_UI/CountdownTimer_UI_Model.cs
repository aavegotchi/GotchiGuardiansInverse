using System;
using PhaseManager.Presenter;

namespace CountdownTimer_UI.Model {
    public class CountdownTimer_UI_Model
    {
        #region Events
        public event Action<bool> OnShowCountdownUIUpdated = delegate {};
        public event Action<float> OnCountdownValueUpdated = delegate {};
        #endregion

        #region Public Variables
        public bool ShowCountdownUI { get { return showCountdownUI; } }
        public float CountdownValue { get { return countdownValue; } }
        #endregion

        #region Private Variables
        private bool showCountdownUI = false;
        private float countdownValue = 0f;
        #endregion

        public CountdownTimer_UI_Model()
        {
            PhasePresenter.Instance.OnShowCoundownUpdated += SetShowCountdownUI;
            PhasePresenter.Instance.OnCountdownValueUpdated += SetCountdownValue;
        }

        private void SetShowCountdownUI(bool isOpen)
        {
            showCountdownUI = isOpen;
            OnShowCountdownUIUpdated(isOpen);
        }

        private void SetCountdownValue(float value)
        {
            countdownValue = value;
            OnCountdownValueUpdated(countdownValue);
        }
    }
}
