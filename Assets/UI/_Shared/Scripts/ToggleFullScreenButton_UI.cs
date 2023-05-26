using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleFullScreenButton_UI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  #region Fields
  [Header("Required Refs")]
  [SerializeField] private Sprite fullScreenSprite = null;
  [SerializeField] private Sprite fullScreenExitSprite = null;
  #endregion

  #region Private Variables
  private Image image = null;
  #endregion

  #region Unity Functions
  private void Awake()
  {
    image = GetComponent<Image>();
  }

  // For acceptable UX, this must be done on pointer down so that it actually occurs on pointer up
  // Reference for more info: (https://docs.unity3d.com/Manual/webgl-cursorfullscreen.html)
  public void OnPointerDown(PointerEventData eventData)
  {
    ToggleFullScreen();
  }

  // For acceptable UX, this must be done on pointer up because this is when full screen toggle actually happens
  // Reference for more info: (https://docs.unity3d.com/Manual/webgl-cursorfullscreen.html)
  public void OnPointerUp(PointerEventData eventData)
  {
    ToggleFullScreenImage();
  }
  #endregion

  #region Private Functions
  private void ToggleFullScreen()
  {
    Screen.fullScreen = !Screen.fullScreen;
  }

  private void ToggleFullScreenImage()
  {
    if (image != null)
    {
      image.sprite = Screen.fullScreen ? fullScreenExitSprite : fullScreenSprite;
    }
  }
  #endregion
}
