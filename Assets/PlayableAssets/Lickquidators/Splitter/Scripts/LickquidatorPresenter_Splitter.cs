using Gotchi.Lickquidator.Presenter;
using Gotchi.Lickquidator.Model;
using UnityEngine;

namespace Gotchi.Lickquidator.Splitter.Presenter
{
    public class LickquidatorPresenter_Splitter : LickquidatorPresenter
    {
        #region Fields
        [SerializeField] GameObject lickquidatorVisual;
        #endregion

        #region Private Variables
        private SplitterJumpManager splitterJumpManager;
        private SplitterJump splitterJump;
        public bool willSplitOnDeath = true;
        #endregion

        #region Unity Functions
        private void Awake()
        {
            splitterJumpManager = SplitterJumpManager.Instance;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            model.OnHealthUpdated += handleOnHealthUpdated;
            lickquidatorVisual.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            model.OnHealthUpdated -= handleOnHealthUpdated;
        }

        #endregion

        #region Public Functions
        public void SetCanSplitOnDeath(bool canSplit)
        {
            willSplitOnDeath = canSplit;
        }

        #endregion

        #region Private Functions
        private void handleOnHealthUpdated() { 
            if(model.Health < 1)
            {
                if(willSplitOnDeath)
                {
                    Debug.Log("splitting");
                    TriggerSplit();
                }
            }
        }

        private void TriggerSplit()
        {
            // Hide visual and stop movement
            lickquidatorVisual.SetActive(false);
            agent.enabled = false;

            // Get SplitterJump from the SplitterJumpManager
            splitterJump = splitterJumpManager.ActivateSplitterJump(this.transform);
        }

        #endregion

        #region Public Functions
        public void DeactivateSplitterAfterJump()
        {
            agent.enabled = true;

            // Return splitterJump to the pool
            splitterJump.gameObject.SetActive(false);
            splitterJump = null; // Clear reference to allow GC and reassignment

            gameObject.SetActive(false);
        }

        public bool IsGoingToSplitOnDeath()
        {
            return willSplitOnDeath;
        }
        #endregion
    }
}

