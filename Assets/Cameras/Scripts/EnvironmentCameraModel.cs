using System;
using Cinemachine;
using GameMaster;
using PhaseManager;
using UnityEngine;

namespace Gotchi.EnvironmentCamera
{
  public class EnvironmentCameraModel : MonoBehaviour
  {
    #region Public Variables
    public bool IsEnabled { get { return isEnabled; } }
    public bool IsZooming { get { return isZooming; } }
    public float MouseScrollDeltaY { get { return mouseScrollDeltaY; } }
    #endregion

    #region Events
    public Action OnIsZoomingUpdated = delegate {};
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject bounds = null;
    public GameObject Bounds { get { return bounds; } }
    [SerializeField] private CinemachineVirtualCamera vCamera = null;
    public CinemachineVirtualCamera VCamera { get { return vCamera; } }

    [Header("Attributes")]
    [SerializeField] private bool isKeyboardMovementEnabled = true;
    public bool IsKeyboardMovementEnabled { get { return isKeyboardMovementEnabled; } }
    [SerializeField] private float keyboardMovementSpeed = 150f;
    public float KeyboardMovementSpeed { get { return keyboardMovementSpeed; } }
    [SerializeField] private bool isEdgeScrollMovementEnabled = true;
    public bool IsEdgeScrollMovementEnabled { get { return isEdgeScrollMovementEnabled; } }
    [SerializeField] private float edgeScrollMovementBuffer = 50f;
    public float EdgeScrollMovementBuffer { get { return edgeScrollMovementBuffer; } }
    [SerializeField] private float edgeScrollMovementSpeedMax = 150f;
    public float EdgeScrollMovementSpeedMax { get { return edgeScrollMovementSpeedMax; } }
    [SerializeField] private float edgeScrollMovementSpeedMin = 25f;
    public float EdgeScrollMovementSpeedMin { get { return edgeScrollMovementSpeedMin; } }
    [SerializeField] private float movementBuffer = 0f;
    public float MovementBuffer { get { return movementBuffer; } }
    [SerializeField] private bool isMouseZoomEnabled = true;
    public bool IsMouseZoomEnabled { get { return isMouseZoomEnabled; } }
    [SerializeField] private float mouseZoomAmount = 50f;
    public float MouseZoomAmount { get { return mouseZoomAmount; } }
    [SerializeField] private float mouseZoomDuration = 1f;
    public float MouseZoomDuration { get { return mouseZoomDuration; } }
    [SerializeField] private float zoomDistanceMax = 260f;
    public float ZoomDistanceMax { get { return zoomDistanceMax; } }
    [SerializeField] private float zoomDistanceMin = 120f;
    public float ZoomDistanceMin { get { return zoomDistanceMin; } }
    [SerializeField] private float zoomAngleMax = 50f;
    public float ZoomAngleMax { get { return zoomAngleMax; } }
    [SerializeField] private float zoomAngleMin = 60f;
    public float ZoomAngleMin { get { return zoomAngleMin; } }
    #endregion

    #region Private Variables
    private bool isEnabled = false;
    private bool isZooming = false;
    private float mouseScrollDeltaY = 0f;
    #endregion

    #region Unity Functions
    void OnEnable()
    {
      GameMasterEvents.PhaseEvents.PhaseChanged += handlePhaseChanged;
    }

    void OnDisable()
    {
      GameMasterEvents.PhaseEvents.PhaseChanged -= handlePhaseChanged;
    }
    #endregion

    #region Public Functions
    public void UpdateIsZooming(float mouseScrollDeltaY)
    {
      if (this.mouseScrollDeltaY == mouseScrollDeltaY) return;

      isZooming = mouseScrollDeltaY != 0f;
      this.mouseScrollDeltaY = mouseScrollDeltaY;
      OnIsZoomingUpdated();
    }
    #endregion

    #region Private Functions
    private void handlePhaseChanged(Phase phase)
    {
      isEnabled = phase == Phase.Prep || phase == Phase.Survival;
    }
    #endregion
  }
}