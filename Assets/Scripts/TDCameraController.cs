using UnityEngine;

public class TDCameraController : MonoBehaviour
{
    #region Fields
    [Header("Attributes")]
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private Vector3 startPosition = new Vector3(7, 86, 55);
    [SerializeField] private Vector3 endPosition = new Vector3(7, 86, 0);
    [SerializeField] private Quaternion startRotation = Quaternion.Euler(60, 180, 0);
    [SerializeField] private Quaternion endRotation = Quaternion.Euler(90, 180, 0);
    #endregion

    #region Unity Functions
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // TODO: use new input system
        {
            transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * panSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, Time.deltaTime * panSpeed);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * panSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * panSpeed);
        }
    }
    #endregion
}
