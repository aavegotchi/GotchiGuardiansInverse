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
            moveAnimation.enabled = true;
            moveAnimation.SetTrigger(((LickquidatorModel_SpeedyBoi)model).MoveAnimTriggerHash);
            moveParticleSystemObj.SetActive(true);
        }

        private void handleOnAttacked()
        {
            StartCoroutine(knockback());
        }

        private IEnumerator knockback()
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos1 = startPos + new Vector3(0f, 0f, -10f);
            Vector3 targetPos2 = startPos + new Vector3(0f, 0f, -8f);
            Vector3 targetPos3 = startPos + new Vector3(0f, 0f, -15f);
            
            float timeFrom0To1 = 0.25f;
            float timeFrom1To2 = 0.08f;
            float timeFrom2To1 = 0.17f;
            float timeFrom1To3 = 0.5f;

            yield return lerpPosition(startPos, targetPos1, timeFrom0To1);
            yield return lerpPosition(targetPos1, targetPos2, timeFrom1To2);
            yield return lerpPosition(targetPos2, targetPos1, timeFrom2To1);
            yield return lerpPosition(targetPos1, targetPos3, timeFrom1To3);
        }

        private IEnumerator lerpPosition(Vector3 startPos, Vector3 endPos, float timeFromStartToEnd)
        {
            float time = 0f;
            while (time < timeFromStartToEnd)
            {
                time = Mathf.MoveTowards(time, timeFromStartToEnd, Time.deltaTime);
                transform.position = Vector3.Lerp(startPos, endPos, time / timeFromStartToEnd);
                yield return null;
            }
        }
        #endregion
    }
}