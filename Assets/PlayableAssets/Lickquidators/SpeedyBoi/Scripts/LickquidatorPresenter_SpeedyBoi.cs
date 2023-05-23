using UnityEngine;
using Gotchi.Lickquidator.Presenter;
using Gotchi.Lickquidator.SpeedyBoi.Model;
using System.Collections;

namespace Gotchi.Lickquidator.SpeedyBoi.Presenter
{
    public class LickquidatorPresenter_SpeedyBoi : LickquidatorPresenter
    {
        #region Fields
        [SerializeField] private Animator moveAnimation = null;
        [SerializeField] private GameObject moveParticleSystemObj = null;
        #endregion

        #region Private Variables
        Vector3 prevPos = Vector3.zero;
        Vector3 dir = Vector3.zero;
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
            // TODO: if we don't like physics, a more sophisticated version of the below coroutine can be considered
            // but using physics would be the cleanest approach

            // StartCoroutine(knockback());
        }

        // void FixedUpdate()
        // {
        //     Vector3 curPos = transform.position;
        //     if (curPos == prevPos) return;
        //     dir = (curPos - prevPos).normalized;
        //     prevPos = curPos;
        // }

        // private IEnumerator knockback()
        // {
        //     Vector3 curDir = -dir;
        //     float duration = 0.5f;
        //     float time = 0f;
        //     while (time < duration) 
        //     {
        //         transform.Translate(curDir * time / duration);
        //         time += Time.deltaTime;
        //         yield return null;  
        //     }
        // }
        #endregion
    }
}