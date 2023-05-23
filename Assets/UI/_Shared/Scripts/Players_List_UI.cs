using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Players_List_UI : MonoBehaviour
{
    #region Fields
    [SerializeField] private List<Player_ListEle_UI> playerElements = new List<Player_ListEle_UI>();
    [SerializeField] private int padding = 10;
    [SerializeField] private bool isShow = false;
    [SerializeField] private float reorderAnimSpeedPerPixel = 0.001f;
    #endregion

    #region Private Variables
    private bool isVisible = false;
    private Sequence visibilityTweener = null;
    private Sequence reorderTweeners = null;
    private Dictionary<string, int> usernameToPlayerElementsIndexDict = new Dictionary<string, int>();
    #endregion

    #region Unity Functions
    void Start()
    {
        foreach(Player_ListEle_UI ele in playerElements)
        {
            if (ele != null) {
                RectTransform rt = ele.GetComponent<RectTransform>();
                rt.localPosition += new Vector3(rt.localScale.x * rt.sizeDelta.x, 0, 0);
            }
        }

        show();
    }

    void Update()
    {
        if (isShow == isVisible) return;

        if (isShow) {
            show();
        }
        else { 
            hide();
        }
    }
    #endregion

    #region Public Functions
    public void AddPlayerEntry(int id, string username, bool isMain = false)
    {
        if (isMain)
        {
            Player_ListEle_UI playerElement = playerElements[0];
            playerElement.SetPlayerName(username);
            playerElement.SetPlayerId(id);
            playerElement.gameObject.SetActive(true);
            usernameToPlayerElementsIndexDict[username] = 0;
            return;
        }

        for (int i=1; i<playerElements.Count; i++)
        {
            Player_ListEle_UI playerElement = playerElements[i];
            if (!playerElement.gameObject.activeSelf)
            {
                playerElement.SetPlayerName(username);
                playerElement.SetPlayerId(id);
                playerElement.gameObject.SetActive(true);
                usernameToPlayerElementsIndexDict[username] = i;
                return;
            }
        }
    }

    public void RemovePlayerEntry(string username)
    {
        int i = usernameToPlayerElementsIndexDict[username];
        playerElements[i].gameObject.SetActive(false);
        usernameToPlayerElementsIndexDict.Remove(username);
    }

    public Player_ListEle_UI GetPlayerEntry(string username)
    {
        int i = usernameToPlayerElementsIndexDict[username];
        return playerElements[i];
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

    #region Private Functions
    private void show()
    {
        if (!isVisible) {
            isShow = true;
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

    private void hide()
    {
        if (isVisible) {
            isShow = false;
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
    #endregion
}
