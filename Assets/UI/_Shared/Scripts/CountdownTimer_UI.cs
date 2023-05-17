using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CountdownTimer_UI : MonoBehaviour
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
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        if (show) {
            canvasGroup.alpha = 1;
        } else {
            canvasGroup.alpha = 0;
        }
    }

    // Update is called once per frame
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
    public void Show() {
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

    public void Hide() {
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

    public void SetTimeLeft(float timeLeft) {

        float animTime = (timeLeft % loopDuration) / loopDuration;
        float newVal = hourglassAnimCurve.Evaluate(animTime);
        hourglassRT.localEulerAngles = new Vector3(0.0f, 0.0f, 360.0f * newVal);

        countdownLabel.SetText(Mathf.Ceil(timeLeft).ToString());
    }
    #endregion
}