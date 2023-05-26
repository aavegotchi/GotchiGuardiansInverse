using System;
using UnityEngine;

namespace Gotchi.UI.FullScreenButton
{
  namespace Model
  {
    public class FullScreenButtonModel : MonoBehaviour
    {
      #region Events
      public event Action OnIsFullScreenUpdated = delegate { };
      #endregion

      #region Fields
      [Header("Required Refs")]
      [SerializeField] private Sprite fullScreenSprite = null;
      [SerializeField] private Sprite fullScreenExitSprite = null;
      #endregion

      #region Private Variables
      private bool isFullScreen = false;
      #endregion

      #region Public Functions
      public void SetIsFullScreen(bool isFullScreen)
      {
        this.isFullScreen = isFullScreen;
        OnIsFullScreenUpdated();
      }

      public Sprite GetFullScreenImageSprite()
      {
        return isFullScreen ? fullScreenExitSprite : fullScreenSprite;
      }
      #endregion
    }
  }
}
