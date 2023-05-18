using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using CountdownTimer_UI.Model;

namespace CountdownTimer_UI {
    namespace Presenter
    {
        public class CountdownTimer_UI_Presenter : MonoBehaviour
        {
            #region Fields
            [Header("Sub Objects")]
            [SerializeField]
            private CanvasGroup canvasGroup;
            [SerializeField]
            private TextMeshProUGUI countdownLabel;
            [SerializeField]
            private RectTransform hourglassRT;

            [Header("Hourglass Animation controls")]
            [SerializeField]
            private float loopDuration = 2.0f;
            [SerializeField]
            private AnimationCurve hourglassAnimCurve;

            [Header("Debug Controls")]
            [SerializeField]
            private bool show = false;
            #endregion

            #region Private Variables
            private bool isVisible = false;
            private Tweener visibilityTween = null;

            private CountdownTimer_UI_Model CountdownTimer_UI_Model = null;
            #endregion

            #region Unity Methods
            void Start()
            {
                if (show) {
                    canvasGroup.alpha = 1;
                } else {
                    canvasGroup.alpha = 0;
                }
                CountdownTimer_UI_Model = new CountdownTimer_UI_Model();
                CountdownTimer_UI_Model.OnShowCountdownUIUpdated += HandleUpdateShowCountdownUI;
                CountdownTimer_UI_Model.OnCountdownValueUpdated += SetTimeLeft;

            }

            void Update()
            {
                if (show != isVisible)
                {
                    if (show)
                    {
                        Show();
                    }
                    else
                    {
                        Hide();
                    }
                }
            }
            #endregion

            #region public functions
            private void Show() {
                if (!isVisible) {
                    show = true;
                    isVisible = true;

                    if (visibilityTween != null)
                    {
                        visibilityTween.Pause();
                        visibilityTween.Kill();
                        visibilityTween = null;
                    }

                    visibilityTween = canvasGroup.DOFade(1.0f, 0.5f);
                    visibilityTween.OnComplete(() => visibilityTween = null);
                }
            }

            private void Hide() {
                if (isVisible) {
                    show = false;
                    isVisible = false;

                    if (visibilityTween != null)
                    {
                        visibilityTween.Pause();
                        visibilityTween.Kill();
                        visibilityTween = null;
                    }

                    visibilityTween = canvasGroup.DOFade(0.0f, 0.5f);
                    visibilityTween.OnComplete(() => visibilityTween = null);
                }
            }

            private void SetTimeLeft(float timeLeft) {

                float animTime = (timeLeft % loopDuration) / loopDuration;
                float newVal = hourglassAnimCurve.Evaluate(animTime);
                hourglassRT.localEulerAngles = new Vector3(0.0f, 0.0f, 360.0f * newVal);

                countdownLabel.SetText(Mathf.Ceil(timeLeft).ToString());
            }
            #endregion

            #region private functions
            public void HandleUpdateShowCountdownUI(bool isOpen)
            {
                if (isOpen) {
                    Show();
                } else {
                    Hide();
                }
            }
            #endregion
        }
    }
}
