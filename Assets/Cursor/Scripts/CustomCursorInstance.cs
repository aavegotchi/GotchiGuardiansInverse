using System;
using UnityEngine;
using GameMaster;

public class CustomCursorInstance : MonoBehaviour
{
    #region Public Variables
    public enum CursorType
    {
        None,
        Default,
        Pan
    }

    public CursorType CurrentCursorType { get { return currentCursorType; } }
    public Texture2D DefaultCursorTexture { get { return defaultCursorTexture; } }
    public Texture2D PanCursorTexture { get { return panCursorTexture; } }
    #endregion

    #region Events
    public Action OnCurrentCursorTypeUpdated = delegate {};
    #endregion

    #region Fields
    [SerializeField] private Texture2D defaultCursorTexture = null;
    [SerializeField] private Texture2D panCursorTexture = null;
    #endregion

    #region Private Variables
    private CursorType currentCursorType = CursorType.None;
    #endregion

    #region Unity Functions
    void OnEnable()
    {
        GameMasterEvents.MouseEvents.OnIsPanning += handleOnIsPanning;
    }

    void OnDisable()
    {
        GameMasterEvents.MouseEvents.OnIsPanning -= handleOnIsPanning;
    }

    void Start()
    {
        SetCursor(CustomCursorInstance.CursorType.Default);
    }
    #endregion

    #region Public Functions
    public void SetCursor(CursorType cursorType)
    {
        currentCursorType = cursorType;
        OnCurrentCursorTypeUpdated();
    }
    #endregion

    #region Event Handlers
    private void handleOnIsPanning(bool isPanning)
    {
        if (isPanning)
        {
            SetCursor(CursorType.Pan);
        }
        else
        {
            SetCursor(CursorType.Default);
        }
    }
    #endregion
}
