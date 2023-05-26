using Gotchi.UI.FullScreenButton.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gotchi.UI.FullScreenButton
{
  namespace Presenter
  {
    public class FullScreenButtonPresenter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
      #region Fields
      [SerializeField] private FullScreenButtonModel model = null;
      #endregion

      #region Private Variables
      private Image image = null;
      #endregion

      #region Unity Functions
      private void Awake()
      {
        image = GetComponent<Image>();
      }

      private void OnEnable()
      {
        model.OnIsFullScreenUpdated += HandleIsFullScreenUpdated;
      }

      private void OnDisable()
      {
        model.OnIsFullScreenUpdated -= HandleIsFullScreenUpdated;
      }

      // Reference for more info: (https://docs.unity3d.com/Manual/webgl-cursorfullscreen.html)
      public void OnPointerDown(PointerEventData _pointerEventData)
      {
        // For acceptable UX, this must be done on pointer down because it actually ends up
        // occurring on pointer up
        Screen.fullScreen = !Screen.fullScreen;
      }

      public void OnPointerUp(PointerEventData _pointerEventData)
      {
        // This is when the full screen change is actually recognized
        model.SetIsFullScreen(Screen.fullScreen);
      }
      #endregion

      #region Private Functions
      private void HandleIsFullScreenUpdated()
      {
        image.sprite = model.GetFullScreenImageSprite();
      }
      #endregion
    }
  }
}
