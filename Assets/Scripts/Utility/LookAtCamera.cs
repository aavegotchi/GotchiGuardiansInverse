using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    #region Public Variables
    [Header("Settings")]
    [SerializeField] private bool faceAwayFromCamera = false;
    #endregion

    #region Private Variables
    private Camera mainCamera = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }
        }

        Vector3 directionToCamera = mainCamera.transform.position - transform.position;

        // Flip the direction if the element should face away from the camera
        if (faceAwayFromCamera)
        {
            directionToCamera = -directionToCamera;
        }

        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera, Vector3.up);
        transform.rotation = Quaternion.Euler(rotationToCamera.eulerAngles.x, 0f, 0f);
    }
    #endregion
}
