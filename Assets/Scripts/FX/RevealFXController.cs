using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class RevealFXController : MonoBehaviour
{
    #region fields
    [SerializeField]
    private List<GameObject> targets;
    [SerializeField]
    private float progress = 0.0f;
    [SerializeField]
    private int botValue = 0;
    [SerializeField]
    private int topValue = 10;
    [SerializeField]
    private bool show = false;
    [SerializeField]
    private float showTime = 1.0f;
    #endregion

    private bool _lastShowApplied = false;
    private Tween showTweener;
    private float _lastAppliedProgress = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject target in targets)
        {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                mat.SetFloat("_RevealOrigin", botValue);
                mat.SetFloat("_RevealDistance", topValue);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastAppliedProgress != progress)
        {
            UpdateProgress();
        }

        if (_lastShowApplied != show)
        {
            UpdateShow();
        }
    }

    private void UpdateShow()
    {
        if (_lastShowApplied == show)
        {
            return;
        }

        _lastShowApplied = show;

        if (showTweener != null)
        {
            showTweener.Pause();
            showTweener.Kill();
            showTweener = null;
        }

        showTweener = DOTween.To(() => progress, (x) => { progress = x; }, show ? 1.0f : 0.0f, showTime);

        foreach (GameObject target in targets)
        {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                mat.SetFloat("_RevealOrigin", botValue);
                mat.SetFloat("_RevealDistance", topValue);
            }
        }
    }

    private void UpdateProgress()
    {
        if (_lastAppliedProgress == progress)
        {
            return;
        }

        if (progress < 0.0f)
        {
            progress = 0.0f;
        }
        else if (progress > 1.0f)
        {
            progress = 1.0f;
        }

        _lastAppliedProgress = progress;

        foreach (GameObject target in targets) {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                mat.SetFloat("_Reveal", _lastAppliedProgress);
            }
        }
    }
}