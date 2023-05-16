namespace CountdownTimer_UI.Model {
    public class CountdownTimer_UI_Model
    {
        public delegate void UpdateShowCoundownUIDel(bool isOpen);
        public event UpdateShowCoundownUIDel UpdateShowCoundownUI;

        public delegate void UpdateCountdownValueDel(float countdownValue);
        public event UpdateCountdownValueDel UpdateCountdownValue;

        private bool ShowCountdownUI = false;
        private float CountdownValue = 0f;

        public CountdownTimer_UI_Model()
        {
            PhaseManager.Instance.OnUpdateShowCountdown += SetShowCountdownUI;
            PhaseManager.Instance.OnUpdateCountdownValue += SetCountdownValue;
        }

        private void SetShowCountdownUI(bool isOpen)
        {
            ShowCountdownUI = isOpen;
            UpdateShowCoundownUI?.Invoke(isOpen);
        }

        public bool GetIsCountdownUIOpen()
        {
            return ShowCountdownUI;
        }

        private void SetCountdownValue(float countdownValue)
        {
            CountdownValue = countdownValue;
            UpdateCountdownValue?.Invoke(countdownValue);
        }

        public float GetCoundownValue()
        {
            return CountdownValue;
        }
    }
}