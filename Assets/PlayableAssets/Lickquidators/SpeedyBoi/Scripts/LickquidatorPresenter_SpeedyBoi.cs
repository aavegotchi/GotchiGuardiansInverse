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

            float time = 0f;
            while (time < timeFrom0To1)
            {
                time = Mathf.MoveTowards(time, timeFrom0To1, Time.deltaTime);
                transform.position = Vector3.Lerp(startPos, targetPos1, time / timeFrom0To1);
                yield return null;
            }
            time = 0f;

            while (time < timeFrom1To2)
            {
                time = Mathf.MoveTowards(time, timeFrom1To2, Time.deltaTime);
                transform.position = Vector3.Lerp(targetPos1, targetPos2, time / timeFrom1To2);
                yield return null;
            }
            time = 0f;

            while (time < timeFrom2To1)
            {
                time = Mathf.MoveTowards(time, timeFrom2To1, Time.deltaTime);
                transform.position = Vector3.Lerp(targetPos2, targetPos1, time / timeFrom2To1);
                yield return null;
            }
            time = 0f;

            while (time < timeFrom1To3)
            {
                time = Mathf.MoveTowards(time, timeFrom1To3, Time.deltaTime);
                transform.position = Vector3.Lerp(targetPos1, targetPos3, time / timeFrom1To3);
                yield return null;
            }
            time = 0f;
        }
        #endregion
    }
}