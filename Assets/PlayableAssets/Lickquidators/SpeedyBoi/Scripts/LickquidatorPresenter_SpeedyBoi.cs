using UnityEngine;
using Gotchi.Lickquidator.Presenter;
using Gotchi.Lickquidator.SpeedyBoi.Model;

namespace Gotchi.Lickquidator.SpeedyBoi.Presenter
{
    public class LickquidatorPresenter_SpeedyBoi : LickquidatorPresenter
    {
        #region Fields
        [SerializeField] private Animator moveAnimation = null;
        [SerializeField] private Animator knockbackAnimation = null;
        [SerializeField] private GameObject moveParticleSystemObj = null;
        #endregion

        #region Unity Functions
        protected override void OnEnable()
        {
            base.OnEnable();
            model.OnIsMovingUpdated += handleOnMovingUpdated;
            model.OnAttacked += handleOnAttacked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            model.OnIsMovingUpdated -= handleOnMovingUpdated;
            model.OnAttacked += handleOnAttacked;
        }
        #endregion

        #region Private Functions
        private void handleOnMovingUpdated()
        {
            if (model.IsMoving)
            {
                moveAnimation.enabled = true;
                moveAnimation.SetTrigger(((LickquidatorModel_SpeedyBoi)model).MoveAnimTriggerHash);
                moveParticleSystemObj.SetActive(true);
            }
            else 
            {
                moveAnimation.enabled = false;
                moveParticleSystemObj.SetActive(false);
            }
        }

        private void handleOnAttacked()
        {
            knockbackAnimation.SetTrigger(((LickquidatorModel_SpeedyBoi)model).KnockbackAnimTriggerHash);
        }
        #endregion
    }
}