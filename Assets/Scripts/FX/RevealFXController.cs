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
    public float Progress = 0.0f;
    [SerializeField]
    private bool AdjustOriginToWorldY = true;
    [SerializeField]
    private float OriginValue = 0.0f;
    [SerializeField]
    private float Distance = 10.0f;
    [SerializeField]
    private bool Show = false;
    [SerializeField]
    private float ShowTime = 1.0f;
    #endregion

    private bool _lastShowApplied = false;
    private Tween showTweener;
    private float _lastAppliedProgress = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        float offset = 0.0f;
        if (AdjustOriginToWorldY)
        {
            offset = transform.position.y;
        }
        foreach (GameObject target in targets)
        {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                float adjustedValue = OriginValue + offset;
                mat.SetFloat("_RevealOrigin", adjustedValue);
                mat.SetFloat("_RevealDistance", Distance);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float adjustedValue = OriginValue;
        if (AdjustOriginToWorldY)
        {
            adjustedValue = OriginValue + transform.position.y;
        }

        foreach (GameObject target in targets)
        {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                mat.SetFloat("_RevealOrigin", adjustedValue);
                mat.SetFloat("_RevealDistance", Distance);
            }
        }

        if (_lastAppliedProgress != Progress)
        {
            UpdateProgress();
        }

        if (_lastShowApplied != Show)
        {
            UpdateShow();
        }
    }

    private void UpdateShow()
    {
        if (_lastShowApplied == Show)
        {
            return;
        }

        _lastShowApplied = Show;

        if (showTweener != null)
        {
            showTweener.Pause();
            showTweener.Kill();
            showTweener = null;
        }

        showTweener = DOTween.To(() => Progress, (x) => { Progress = x; }, Show ? 1.0f : 0.0f, ShowTime);

        float newOrigin = OriginValue;
        if (AdjustOriginToWorldY)
        {
            newOrigin = OriginValue + transform.position.y;
        }

        foreach (GameObject target in targets)
        {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                mat.SetFloat("_RevealOrigin", newOrigin);
                mat.SetFloat("_RevealDistance", Distance);
            }
        }
    }

    private void UpdateProgress()
    {
        if (_lastAppliedProgress == Progress)
        {
            return;
        }

        if (Progress < 0.0f)
        {
            Progress = 0.0f;
        }
        else if (Progress > 1.0f)
        {
            Progress = 1.0f;
        }

        _lastAppliedProgress = Progress;

        foreach (GameObject target in targets) {
            foreach (Material mat in target.GetComponent<Renderer>().materials)
            {
                mat.SetFloat("_Reveal", _lastAppliedProgress);
            }
        }
    }
}
