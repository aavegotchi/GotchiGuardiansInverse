using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Players_List_UI : MonoBehaviour
{
    #region fields
    [SerializeField]
    private List<Player_ListEle_UI> playerElements;
    [SerializeField]
    private Player_ListEle_UI player;
    [SerializeField]
    private int padding = 10;
    [SerializeField]
    private bool show = false;
    [SerializeField]
    private float reorderAnimSpeedPerPixel = 0.001f;
    #endregion

    #region private variables
    bool isVisible = false;
    Sequence visibilityTweener = null;
    Sequence reorderTweeners = null;
    #endregion

    #region unity functions
    // Start is called before the first frame update
    void Start()
    {
        foreach(Player_ListEle_UI ele in playerElements)
        {
            if (ele != null) {
                RectTransform rt = ele.GetComponent<RectTransform>();
                rt.localPosition += new Vector3(rt.localScale.x * rt.sizeDelta.x, 0, 0);
            }
        }

        Show();
    }

    // Update is called once per frame
    void Update()
    {
        if (show != isVisible)
        {
            if (show) {
                Show();
            }
            else { 
                Hide();
            }
        }
    }
    #endregion

    #region public functions
    public void Show()
    {
        if (!isVisible) {
            show = true;
            isVisible = true;

            if (visibilityTweener != null)
            {
                visibilityTweener.Pause();
                visibilityTweener.Kill();
                visibilityTweener = null;
            }

            visibilityTweener = DOTween.Sequence();
            visibilityTweener.AppendCallback(() => visibilityTweener = null);

            float offset = 0.0f;

            foreach (Player_ListEle_UI ele in playerElements)
            {
                RectTransform rt = ele.GetComponent<RectTransform>();
                visibilityTweener.Insert(offset, DOTween.To(() => rt.localPosition.x, (x) => rt.localPosition = new Vector3(x, rt.localPosition.y, rt.localPosition.z), 0, 0.4f));
                offset += 0.1f;
            }
        }
    }

    public void Hide()
    {
        if (isVisible) {
            show = false;
            isVisible = false;

            if (visibilityTweener != null)
            {
                visibilityTweener.Pause();
                visibilityTweener.Kill();
                visibilityTweener = null;
            }

            visibilityTweener = DOTween.Sequence();
            visibilityTweener.AppendCallback(() => visibilityTweener = null);

            foreach (Player_ListEle_UI ele in playerElements)
            {
                RectTransform rt = ele.GetComponent<RectTransform>();
                visibilityTweener.Insert(0.0f, DOTween.To(() => rt.localPosition.x, (x) => rt.localPosition = new Vector3(x, rt.localPosition.y, rt.localPosition.z), rt.localScale.x * rt.sizeDelta.x, 0.4f));
            }
        }
    }

    public void CheckAndUpdatePlayerOrder()
    {
        playerElements = playerElements.OrderByDescending((item) => item.CurrentHP).ToList();

        int currentIndex = playerElements.Count - 1;
        float yPos = 0.0f;

        if (reorderTweeners != null)
        {
            reorderTweeners.Pause();
            reorderTweeners.Kill();
            reorderTweeners = null;
        }

        foreach (Player_ListEle_UI ele in playerElements)
        {
            if (currentIndex != ele.transform.GetSiblingIndex())
            {
                ele.transform.SetSiblingIndex(currentIndex);
            }

            RectTransform rt = ele.GetComponent<RectTransform>();

            if (rt.localPosition.y != yPos)
            {
                if (reorderTweeners == null)
                {
                    reorderTweeners = DOTween.Sequence();
                    reorderTweeners.AppendCallback(() => reorderTweeners = null);
                }

                reorderTweeners.Insert(0.0f, DOTween.To(() => rt.localPosition.y, 
                                                        (y) => rt.localPosition = new Vector3(rt.localPosition.x, y, rt.localPosition.z), 
                                                        yPos, reorderAnimSpeedPerPixel * Mathf.Abs(yPos - rt.localPosition.y)));
            }

            yPos -= rt.localScale.y * rt.sizeDelta.y + padding;
            --currentIndex;
        }
    }
        #endregion
}
