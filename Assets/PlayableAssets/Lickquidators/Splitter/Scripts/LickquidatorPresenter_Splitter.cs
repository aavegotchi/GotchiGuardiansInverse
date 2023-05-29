using Gotchi.Lickquidator.Presenter;
using Gotchi.Lickquidator.Splitter.Model;
using UnityEngine;

namespace Gotchi.Lickquidator.Splitter.Presenter
{
    public class LickquidatorPresenter_Splitter : LickquidatorPresenter
    {
        #region Private Variables
        private SplitterJumpManager splitterJumpManager;
        private SplitterJump splitterJump;
        public bool willSplitOnDeath = true;
        #endregion

        #region Unity Functions
        protected override void Awake()
        {
            base.Awake();
            splitterJumpManager = SplitterJumpManager.Instance;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            model.OnHealthUpdated += handleOnHealthUpdated;
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
            // Get SplitterJump from the SplitterJumpManager
            splitterJump = splitterJumpManager.ActivateSplitterJump(this.transform, model.EnemyBlueprint);
        }

        #endregion

        #region Public Functions
        public void DeactivateSplitterAfterJump()
        {
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

