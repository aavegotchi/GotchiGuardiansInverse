using Cinemachine;
using GameMaster;
using System.Collections;
using UnityEngine;

namespace Gotchi.EnvironmentCamera
{
  public class EnvironmentCameraController : MonoBehaviour
  {
    #region Fields
    [SerializeField] private EnvironmentCameraModel model = null;
    #endregion

    #region Private Variables
    private Camera mainCamera = null;
    private CinemachineTransposer vCameraTransposer = null;
    private Vector3 boundsMax = Vector3.positiveInfinity;
    private Vector3 boundsMin = Vector3.negativeInfinity;
    private Vector3 cameraPositionOffset = Vector3.zero;
    private float zoomTime = 0f;
    private Vector3 defaultCameraPosition = Vector3.zero;
    private Vector3 defaultCameraPositionOffset = Vector3.zero;
    private bool isPanning = false;
    #endregion

    #region Unity Functions
    void Awake()
    {
      setBounds();

      mainCamera = Camera.main;
      defaultCameraPosition = transform.position;
      vCameraTransposer = model.VCamera.GetCinemachineComponent<CinemachineTransposer>();
      cameraPositionOffset = vCameraTransposer.m_FollowOffset;
    }

    void OnEnable()
    {
      model.OnIsZoomingUpdated += handleOnIsZoomingUpdated;
    }

    void OnDisable()
    {
      model.OnIsZoomingUpdated -= handleOnIsZoomingUpdated;
    }

    void Update()
    {
      if (!model.IsEnabled) return;

      Vector3 movementInputDirection = getMovementInput();
      if (movementInputDirection != Vector3.zero)
      {
        handleMovementInput(movementInputDirection);

        if (!isPanning)
        {
          isPanning = true;
          GameMasterEvents.MouseEvents.OnIsPanning(isPanning);
        }
      }
      else if (isPanning)
      {
        isPanning = false;
        GameMasterEvents.MouseEvents.OnIsPanning(isPanning);
      }

      if (!model.IsZooming)
      {
        handleZoomInput();
      }
    }
    #endregion

    #region Public Functions
    public void Reset()
    {
      vCameraTransposer.m_FollowOffset = new Vector3(0f, 199.17f, -167.12f); // Hardcoded from zoom calculation
      transform.position = defaultCameraPosition;
    }
    #endregion

    #region Event Handlers
    private void handleOnIsZoomingUpdated()
    {
      if (model.MouseScrollDeltaY == 0) return;

      StartCoroutine(zoomAndMoveCamera());
    }
    #endregion

    #region Private Functions
    private void setBounds()
    {
      Renderer boundsRenderer = model.Bounds.GetComponent<Renderer>();

      if (boundsRenderer == null)
      {
        Renderer[] childRenderers = model.Bounds.GetComponentsInChildren<Renderer>();

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

      Vector3 boundsBufferVector = new Vector3(model.MovementBuffer, model.MovementBuffer, model.MovementBuffer);

      boundsMax = boundsRenderer.bounds.max + boundsBufferVector;
      boundsMin = boundsRenderer.bounds.min - boundsBufferVector;
    }

    private Vector3 getMovementInput()
    {
      Vector3 movementInputDirection = new Vector3(0, 0, 0);

      // Keyboard movement
      if (model.IsKeyboardMovementEnabled)
      {
        // Up
        if (Input.GetKey(KeyCode.W))
        {
          movementInputDirection.z += model.KeyboardMovementSpeed;
        }

        // Down
        if (Input.GetKey(KeyCode.S))
        {
          movementInputDirection.z -= model.KeyboardMovementSpeed;
        }

        // Left
        if (Input.GetKey(KeyCode.A))
        {
          movementInputDirection.x -= model.KeyboardMovementSpeed;
        }

        // Right
        if (Input.GetKey(KeyCode.D))
        {
          movementInputDirection.x += model.KeyboardMovementSpeed;
        }
      }

      // Edge scrolling movement
      if (model.IsEdgeScrollMovementEnabled)
      {
        Vector3 mouseViewportPoint = mainCamera.ScreenToViewportPoint(Input.mousePosition);

        bool isMouseOnScreen = !(
          mouseViewportPoint.x < 0 ||
          mouseViewportPoint.x > 1 ||
          mouseViewportPoint.y < 0 ||
          mouseViewportPoint.y > 1
        );

        if (isMouseOnScreen)
        {
          float mousePositionX = Input.mousePosition.x;
          float mousePositionY = Input.mousePosition.y;

          // Up
          float upBreakpoint = Screen.height - model.EdgeScrollMovementBuffer;
          if (mousePositionY >= upBreakpoint)
          {
            float movementSpeedStrength = (mousePositionY - upBreakpoint) / model.EdgeScrollMovementBuffer;
            movementInputDirection.z += getEdgeScrollMovementSpeed(movementSpeedStrength);
          }

          // Down
          float downBreakpoint = model.EdgeScrollMovementBuffer;
          if (mousePositionY <= downBreakpoint)
          {
            float movementSpeedStrength = (downBreakpoint - mousePositionY) / model.EdgeScrollMovementBuffer;
            movementInputDirection.z -= getEdgeScrollMovementSpeed(movementSpeedStrength);
          }

          // Left
          float leftBreakpoint = model.EdgeScrollMovementBuffer;
          if (mousePositionX <= leftBreakpoint)
          {
            float movementSpeedStrength = (leftBreakpoint - mousePositionX) / model.EdgeScrollMovementBuffer;
            movementInputDirection.x -= getEdgeScrollMovementSpeed(movementSpeedStrength);
          }

          // Right
          float rightBreakpoint = Screen.width - model.EdgeScrollMovementBuffer;
          if (mousePositionX >= rightBreakpoint)
          {
            float movementSpeedStrength = (mousePositionX - rightBreakpoint) / model.EdgeScrollMovementBuffer;
            movementInputDirection.x += getEdgeScrollMovementSpeed(movementSpeedStrength);
          }
        }
      }

      return movementInputDirection;
    }

    private void handleMovementInput(Vector3 movementInputDirection)
    {
      // Clamp movement speed
      float movementSpeedMax = Mathf.Max(model.KeyboardMovementSpeed, model.EdgeScrollMovementSpeedMax);
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

    private float getEdgeScrollMovementSpeed(float movementSpeedStrength)
    {
      return (model.EdgeScrollMovementSpeedMax - model.EdgeScrollMovementSpeedMin) * movementSpeedStrength + model.EdgeScrollMovementSpeedMin;
    }

    private void handleZoomInput()
    {
      if (model.IsMouseZoomEnabled)
      {
        float mouseScrollDeltaY = Input.mouseScrollDelta.y;
        model.UpdateIsZooming(mouseScrollDeltaY);
      }
    }

    private Vector3 getMousePosition()
    {
      Vector3 mousePosition = Vector3.zero;
      Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit))
      {
        mousePosition = hit.point;
      }
      return mousePosition;
    }

    private IEnumerator zoomAndMoveCamera()
    {
      // Zoom original position
      Vector3 origCameraPositionOffset = vCameraTransposer.m_FollowOffset;
      Vector3 origCameraPosition = transform.position;
      Vector3 newCameraPosition = defaultCameraPosition;
      if (model.MouseScrollDeltaY > 0)
      {
        newCameraPosition = getMousePosition();
        newCameraPosition.y = transform.position.y;
      }

      while (zoomTime < model.MouseZoomDuration)
      {
        Vector3 cameraDirection = cameraPositionOffset.normalized;

        if (model.MouseScrollDeltaY > 0)
        {
          // In
          cameraPositionOffset -= cameraDirection * model.MouseZoomAmount;
        }
        if (model.MouseScrollDeltaY < 0)
        {
          // Out
          cameraPositionOffset += cameraDirection * model.MouseZoomAmount;
        }

        // Clamp zoom distance
        if (cameraPositionOffset.magnitude > model.ZoomDistanceMax)
        {
          cameraPositionOffset = cameraDirection * model.ZoomDistanceMax;
        }
        if (cameraPositionOffset.magnitude < model.ZoomDistanceMin)
        {
          cameraPositionOffset = cameraDirection * model.ZoomDistanceMin;
        }

        // Change angle based on zoom level
        float currentZoomPercent = (cameraPositionOffset.magnitude - model.ZoomDistanceMin) / (model.ZoomDistanceMax - model.ZoomDistanceMin);
        float newCameraAngle = (model.ZoomAngleMax - model.ZoomAngleMin) * currentZoomPercent + model.ZoomAngleMin;

        Vector3 newCameraPositionOffset = Quaternion.Euler(newCameraAngle, 0, 0) * -transform.forward * cameraPositionOffset.magnitude;

        newCameraPositionOffset = Vector3.Lerp(
          origCameraPositionOffset,
          newCameraPositionOffset,
          zoomTime / model.MouseZoomDuration
        );
        vCameraTransposer.m_FollowOffset = newCameraPositionOffset;

        //Debug.Log("-----newCameraPositionOffset: " + newCameraPositionOffset.ToString());

        // Move camera to either mouse point or default
        transform.position = Vector3.Lerp(
          origCameraPosition,
          newCameraPosition,
          zoomTime / model.MouseZoomDuration
        );

        zoomTime += Time.deltaTime;

        yield return null;
      }

      zoomTime = 0f;
      model.UpdateIsZooming(0f);
    }
    #endregion
  }
}
