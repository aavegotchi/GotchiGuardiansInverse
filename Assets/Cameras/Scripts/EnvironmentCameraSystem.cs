using Cinemachine;
using UnityEditor;
using UnityEngine;

public class EnvironmentCameraSystem : MonoBehaviour {
  #region Fields
  [Header("Required Refs")]
  [SerializeField] private GameObject bounds = null;

  [Header("Attributes")]
  [SerializeField] private bool isKeyboardMovementEnabled = true;
  [SerializeField] private float keyboardMovementSpeed = 25f;
  
  [SerializeField] private bool isEdgeScrollMovementEnabled = true;
  [SerializeField] private float edgeScrollBuffer = 50f;
  [SerializeField] private float edgeScrollMovementSpeed = 100f;
  
  [SerializeField] private float boundsBuffer = 2.5f;
  #endregion

  #region Private Variables
  private Vector3 boundsMax = new Vector3(+Mathf.Infinity, +Mathf.Infinity, +Mathf.Infinity);
  private Vector3 boundsMin = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
  #endregion

  #region Unity Functions
  private void Awake() {
    SetBounds();
  }

  private void Update() {
    HandleMovement();
  }
  #endregion

  #region Private Functions
  private void SetBounds() {
    Renderer boundsRenderer = bounds.GetComponent<Renderer>();

    if (boundsRenderer == null) {
      Renderer[] childRenderers = bounds.GetComponentsInChildren<Renderer>();

      if (childRenderers.Length > 0) {
        boundsRenderer = childRenderers[0];
      
        for (int i = 1; i < childRenderers.Length; i++) {
          boundsRenderer.bounds.Encapsulate(childRenderers[i].bounds);
        }
      }
    }

    if (boundsRenderer == null) {
      return;
    }

    Vector3 boundsBufferVector = new Vector3(boundsBuffer, boundsBuffer, boundsBuffer);
    
    boundsMax = boundsRenderer.bounds.max + boundsBufferVector;
    boundsMin = boundsRenderer.bounds.min - boundsBufferVector;
  }

  private void HandleMovement() {
    Vector3 movementInputDirection = new Vector3(0, 0, 0);

    if (isKeyboardMovementEnabled) {
      if (Input.GetKey(KeyCode.W)) {
        movementInputDirection.z += keyboardMovementSpeed;
      }
      if (Input.GetKey(KeyCode.S)) {
        movementInputDirection.z -= keyboardMovementSpeed;
      }
      if (Input.GetKey(KeyCode.A)) {
        movementInputDirection.x -= keyboardMovementSpeed;
      }
      if (Input.GetKey(KeyCode.D)) {
        movementInputDirection.x += keyboardMovementSpeed;
      }
    }

    if (isEdgeScrollMovementEnabled) {
      Vector3 mouseViewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
      bool isMouseOnScreen = !(
        mouseViewportPoint.x < 0 ||
        mouseViewportPoint.x > 1 ||
        mouseViewportPoint.y < 0 ||
        mouseViewportPoint.y > 1
      );
      
      if (isMouseOnScreen) {
        if (Input.mousePosition.x <= edgeScrollBuffer) {
          movementInputDirection.x -= edgeScrollMovementSpeed;
        }
        if (Input.mousePosition.y <= edgeScrollBuffer) {
          movementInputDirection.z -= edgeScrollMovementSpeed;
        }
        if (Input.mousePosition.x >= Screen.width - edgeScrollBuffer) {
          movementInputDirection.x += edgeScrollMovementSpeed;
        }
        if (Input.mousePosition.y >= Screen.height - edgeScrollBuffer) {
          movementInputDirection.z += edgeScrollMovementSpeed;
        }
      }
    }

    Vector3 movementDirection = transform.forward * movementInputDirection.z + transform.right * movementInputDirection.x;

    Vector3 newPosition = transform.position + (movementDirection * Time.deltaTime);

    newPosition = new Vector3(
      Mathf.Clamp(newPosition.x, boundsMin.x, boundsMax.x),
      transform.position.y,
      Mathf.Clamp(newPosition.z, boundsMin.z, boundsMax.z)
    );

    transform.position = newPosition;
  }
  #endregion
}
