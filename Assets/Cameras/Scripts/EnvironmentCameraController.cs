using Cinemachine;
using GameMaster;
using PhaseManager;
using UnityEngine;

public class EnvironmentCameraController : MonoBehaviour
{
  #region Fields
  [Header("Required Refs")]
  [SerializeField] private GameObject bounds = null;
  [SerializeField] private CinemachineVirtualCamera vCamera = null;

  [Header("Attributes")]
  [SerializeField] private bool isKeyboardMovementEnabled = true;
  [SerializeField] private float keyboardMovementSpeed = 150f;

  [SerializeField] private bool isEdgeScrollMovementEnabled = true;
  [SerializeField] private float edgeScrollMovementBuffer = 50f;
  [SerializeField] private float edgeScrollMovementSpeedMax = 150f;
  [SerializeField] private float edgeScrollMovementSpeedMin = 25f;

  [SerializeField] private float movementBuffer = 0f;

  [SerializeField] private bool isMouseZoomEnabled = true;
  [SerializeField] private float mouseZoomAmount = 50f;
  [SerializeField] private float mouseZoomSpeed = 5f;

  [SerializeField] private float zoomDistanceMax = 300f;
  [SerializeField] private float zoomDistanceMin = 125f;
  [SerializeField] private float zoomAngleMax = 65f;
  [SerializeField] private float zoomAngleMin = 80f;
  #endregion

  #region Private Variables
  private bool isEnabled = false;

  private Vector3 boundsMax = Vector3.positiveInfinity;
  private Vector3 boundsMin = Vector3.negativeInfinity;

  private CinemachineTransposer vCameraTransposer = null;

  private Vector3 cameraPositionOffset = Vector3.zero;
  #endregion

  #region Unity Functions
  private void Awake()
  {
    SetBounds();

    vCameraTransposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
    cameraPositionOffset = vCameraTransposer.m_FollowOffset;
  }

  private void OnEnable()
  {
    GameMasterEvents.PhaseEvents.PhaseChanged += HandlePhaseChanged;
  }

  private void OnDisable()
  {
    GameMasterEvents.PhaseEvents.PhaseChanged -= HandlePhaseChanged;
  }

  private void Update()
  {
    if (isEnabled)
    {
      HandleMovement();
      HandleZoom();
    }
  }
  #endregion

  #region Private Functions
  private void SetBounds()
  {
    Renderer boundsRenderer = bounds.GetComponent<Renderer>();

    if (boundsRenderer == null)
    {
      Renderer[] childRenderers = bounds.GetComponentsInChildren<Renderer>();

      if (childRenderers.Length > 0)
      {
        boundsRenderer = childRenderers[0];

        for (int i = 1; i < childRenderers.Length; i++)
        {
          boundsRenderer.bounds.Encapsulate(childRenderers[i].bounds);
        }
      }
    }

    if (boundsRenderer == null)
    {
      return;
    }

    Vector3 boundsBufferVector = new Vector3(movementBuffer, movementBuffer, movementBuffer);

    boundsMax = boundsRenderer.bounds.max + boundsBufferVector;
    boundsMin = boundsRenderer.bounds.min - boundsBufferVector;
  }

  private void HandlePhaseChanged(Phase phase)
  {
    isEnabled = phase == Phase.Prep || phase == Phase.Survival;
  }

  private void HandleMovement()
  {
    Vector3 movementInputDirection = new Vector3(0, 0, 0);

    // Keyboard movement
    if (isKeyboardMovementEnabled)
    {
      // Up
      if (Input.GetKey(KeyCode.W))
      {
        movementInputDirection.z += keyboardMovementSpeed;
      }

      // Down
      if (Input.GetKey(KeyCode.S))
      {
        movementInputDirection.z -= keyboardMovementSpeed;
      }

      // Left
      if (Input.GetKey(KeyCode.A))
      {
        movementInputDirection.x -= keyboardMovementSpeed;
      }

      // Right
      if (Input.GetKey(KeyCode.D))
      {
        movementInputDirection.x += keyboardMovementSpeed;
      }
    }

    // Edge scrolling movement
    if (isEdgeScrollMovementEnabled)
    {
      Vector3 mouseViewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);

      bool isMouseOnScreen = !(
        mouseViewportPoint.x < 0 ||
        mouseViewportPoint.x > 1 ||
        mouseViewportPoint.y < 0 ||
        mouseViewportPoint.y > 1
      );

      if (isMouseOnScreen)
      {
        // Up
        float upBreakpoint = Screen.height - edgeScrollMovementBuffer;
        if (Input.mousePosition.y >= upBreakpoint)
        {
          float movementSpeedStrength = (Input.mousePosition.y - upBreakpoint) / edgeScrollMovementBuffer;
          movementInputDirection.z += GetEdgeScrollMovementSpeed(movementSpeedStrength);
        }

        // Down
        float downBreakpoint = edgeScrollMovementBuffer;
        if (Input.mousePosition.y <= downBreakpoint)
        {
          float movementSpeedStrength = (downBreakpoint - Input.mousePosition.y) / edgeScrollMovementBuffer;
          movementInputDirection.z -= GetEdgeScrollMovementSpeed(movementSpeedStrength);
        }

        // Left
        float leftBreakpoint = edgeScrollMovementBuffer;
        if (Input.mousePosition.x <= leftBreakpoint)
        {
          float movementSpeedStrength = (leftBreakpoint - Input.mousePosition.x) / edgeScrollMovementBuffer;
          movementInputDirection.x -= GetEdgeScrollMovementSpeed(movementSpeedStrength);
        }

        // Right
        float rightBreakpoint = Screen.width - edgeScrollMovementBuffer;
        if (Input.mousePosition.x >= rightBreakpoint)
        {
          float movementSpeedStrength = (Input.mousePosition.x - rightBreakpoint) / edgeScrollMovementBuffer;
          movementInputDirection.x += GetEdgeScrollMovementSpeed(movementSpeedStrength);
        }
      }
    }

    // Clamp movement speed
    float movementSpeedMax = Mathf.Max(keyboardMovementSpeed, edgeScrollMovementSpeedMax);
    movementInputDirection.z = Mathf.Clamp(movementInputDirection.z, -movementSpeedMax, movementSpeedMax);
    movementInputDirection.x = Mathf.Clamp(movementInputDirection.x, -movementSpeedMax, movementSpeedMax);

    Vector3 movementDirection = transform.forward * movementInputDirection.z + transform.right * movementInputDirection.x;

    Vector3 newPosition = transform.position + (Time.deltaTime * movementDirection);

    newPosition = new Vector3(
      Mathf.Clamp(newPosition.x, boundsMin.x, boundsMax.x),
      transform.position.y,
      Mathf.Clamp(newPosition.z, boundsMin.z, boundsMax.z)
    );

    transform.position = newPosition;
  }

  private float GetEdgeScrollMovementSpeed(float movementSpeedStrength)
  {
    return (edgeScrollMovementSpeedMax - edgeScrollMovementSpeedMin) * movementSpeedStrength + edgeScrollMovementSpeedMin;
  }

  private void HandleZoom()
  {
    Vector3 cameraDirection = cameraPositionOffset.normalized;

    if (isMouseZoomEnabled)
    {
      if (Input.mouseScrollDelta.y > 0)
      {
        // In
        cameraPositionOffset -= cameraDirection * mouseZoomAmount;
      }
      if (Input.mouseScrollDelta.y < 0)
      {
        // Out
        cameraPositionOffset += cameraDirection * mouseZoomAmount;
      }
    }

    // Clamp zoom distance
    if (cameraPositionOffset.magnitude > zoomDistanceMax)
    {
      cameraPositionOffset = cameraDirection * zoomDistanceMax;
    }
    if (cameraPositionOffset.magnitude < zoomDistanceMin)
    {
      cameraPositionOffset = cameraDirection * zoomDistanceMin;
    }

    // Change angle based on zoom level
    float currentZoomPercent = (cameraPositionOffset.magnitude - zoomDistanceMin) / (zoomDistanceMax - zoomDistanceMin);
    float newCameraAngle = (zoomAngleMax - zoomAngleMin) * currentZoomPercent + zoomAngleMin;

    Vector3 newCameraPositionOffset = Quaternion.Euler(newCameraAngle, 0, 0) * -transform.forward * cameraPositionOffset.magnitude;

    newCameraPositionOffset = Vector3.Lerp(
      vCameraTransposer.m_FollowOffset,
      newCameraPositionOffset,
      Time.deltaTime * mouseZoomSpeed
    );

    vCameraTransposer.m_FollowOffset = newCameraPositionOffset;
  }
  #endregion
}
