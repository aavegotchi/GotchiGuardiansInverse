using UnityEngine;

namespace Gotchi.EnvironmentCamera
{
    public class EnvironmentCameraTrigger : MonoBehaviour
  {
    [SerializeField] private EnvironmentCameraController environmentCameraController = null;

    void OnEnable()
    {
        environmentCameraController.Reset();
    }
  }
}