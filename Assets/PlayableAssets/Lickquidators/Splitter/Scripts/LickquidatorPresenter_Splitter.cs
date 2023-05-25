using Gotchi.Lickquidator.Presenter;
using Gotchi.Lickquidator.Model;
using UnityEngine;

namespace Gotchi.Lickquidator.Splitter.Presenter
{
    public class LickquidatorPresenter_Splitter : LickquidatorPresenter
    {
        #region Fields
        [SerializeField] GameObject lickquidatorVisual;
        [SerializeField] SplitterJump splitterJump;
        #endregion

        #region Private Variables

        private bool willSplitOnDeath = true;
        #endregion

        #region Unity Functions
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

        private void TriggerSplit() {
            // Hide visual and stop movement
            lickquidatorVisual.SetActive(false);
            agent.enabled = false;

            if(splitterJump != null)
            {
                splitterJump.gameObject.SetActive(true);
            }
        }
        #endregion

        #region Public Functions
        public void DeactivateSplitterAfterJump()
        {
            agent.enabled = true;
            gameObject.SetActive(false);
        }

        public bool IsGoingToSplitOnDeath()
        {
            return willSplitOnDeath;
        }
        #endregion
    }
}

