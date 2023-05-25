using UnityEngine;

public class CustomCursorController : MonoBehaviour
{
    #region Fields
    [SerializeField] private CustomCursorInstance instance = null;
    #endregion

    #region Unity Functions
    void OnEnable()
    {
        instance.OnCurrentCursorTypeUpdated += handleOnCurrentCursorTypeUpdated;
    }   

    void OnDisable()
    {
        instance.OnCurrentCursorTypeUpdated -= handleOnCurrentCursorTypeUpdated;
    }
    #endregion

    #region Private Functions
    private void handleOnCurrentCursorTypeUpdated()
    {
        Texture2D cursorTexture = instance.CurrentCursorType == CustomCursorInstance.CursorType.Default
            ? instance.DefaultCursorTexture
            : instance.PanCursorTexture;

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }
    #endregion
}
