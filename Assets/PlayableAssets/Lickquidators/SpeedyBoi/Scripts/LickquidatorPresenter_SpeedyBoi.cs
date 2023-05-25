using UnityEngine;
using Gotchi.Lickquidator.Presenter;
using Gotchi.Lickquidator.SpeedyBoi.Model;

namespace Gotchi.Lickquidator.SpeedyBoi.Presenter
{
    public class LickquidatorPresenter_SpeedyBoi : LickquidatorPresenter
    {
        #region Fields
        [SerializeField] private Animator moveAnimation = null;
        [SerializeField] private GameObject moveParticleSystemObj = null;
        #endregion

        #region Unity Functions
        protected override void OnEnable()
        {
            base.OnEnable();
            model.OnAttacked += handleOnAttacked;
            showMovingEffects();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            model.OnAttacked += handleOnAttacked;
        }
        #endregion

        #region Private Functions
        private void showMovingEffects()
        {
            moveAnimation.SetTrigger(((LickquidatorModel_SpeedyBoi)model).MoveAnimTriggerHash);
            moveParticleSystemObj.SetActive(true);
        }

        private void handleOnAttacked()
        {
            Vector3 dir = GetDirectionToTarget();
            rigidBody.AddForce(dir.normalized * -200f, ForceMode.Impulse); 
        }
        #endregion
    }
}