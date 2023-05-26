using System;
using UnityEngine;

namespace Gotchi.UI.FullScreenButton
{
  namespace Model
  {
    public class FullScreenButtonModel : MonoBehaviour
    {
      #region Public Variables
      public bool IsFullScreen { get { return isFullScreen; } }
      #endregion

      #region Events
      public event Action OnIsFullScreenUpdated = delegate { };
      #endregion

      #region Fields
      [Header("Required Refs")]
      [SerializeField] private Sprite fullScreenSprite = null;
      public Sprite FullScreenSprite { get { return fullScreenSprite; } }

      [SerializeField] private Sprite fullScreenExitSprite = null;
      public Sprite FullScreenExitSprite { get { return fullScreenExitSprite; } }
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
        return IsFullScreen ? FullScreenExitSprite : FullScreenSprite;
      }
      #endregion
    }
  }
}
