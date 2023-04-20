using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ProgressBar_UI : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private Image background = null;
    [SerializeField] private Image progressBar = null;
    [SerializeField] private Animator progressBarAnimator = null;
    #endregion

    #region Private Variables
    private float progressTimer = 0f;
    private float currentProgress = 0f;
    private bool progressStarted = false;
    private Action callback = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        Assert.IsNotNull(background);
        Assert.IsNotNull(progressBar);
        Assert.IsNotNull(progressBarAnimator);
    }

    void Update()
    {
        if (progressStarted && currentProgress < 1f)
        {
            updateProgressBar();
        }
    }
    #endregion

    #region Public Functions
    public void ShowProgressBarAndSetDuration(float duration, Action cb)
    {
        callback = cb;
        progressBarAnimator.SetTrigger("Show");
        StartCoroutine(delayStartUntilAfterAnimation(duration));
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(ProgressBarManager.Instance.gameObject.transform, true);
        progressBar.fillAmount = 0f;
    }
    #endregion

    #region Private Functions
    private IEnumerator delayStartUntilAfterAnimation(float duration)
    {
        yield return new WaitForSeconds(1f);

        progressTimer = duration;
        currentProgress = 0f;
        progressStarted = true;
    }

    private void updateProgressBar()
    {
        currentProgress += Time.deltaTime / progressTimer;
        progressBar.fillAmount = currentProgress;

        if (currentProgress >= 1f)
        {
            progressStarted = false;
            progressBarAnimator.SetTrigger("Hide");
            callback();
        }
    }
    #endregion
}
