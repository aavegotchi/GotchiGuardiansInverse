using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleFullScreenButton_UI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  #region Unity Functions
  public void OnPointerDown(PointerEventData eventData)
  {
    Debug.Log($"[OnPointerDown]");
    ToggleFullScreen();
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    Debug.Log($"[OnPointerUp]");
  }
  #endregion

  #region Private Functions
  private void ToggleFullScreen()
  {
    Screen.fullScreen = !Screen.fullScreen;
  }
  #endregion
}
