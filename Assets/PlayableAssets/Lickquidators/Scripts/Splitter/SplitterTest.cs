using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SplitterTest : MonoBehaviour
{
    #region Events
    // Event declarations go here, if any
    #endregion

    #region Public Variables
    // None
    #endregion

    #region Fields
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float initialSearchRadius = 10f;
    [SerializeField] private float minJumpDistance = 9f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float coneAngle = 10;
    #endregion

    #region Private Variables
    private Vector3 nextTarget;
    private SplitterController splitterController;
    private const float TargetThreshold = .2f;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        splitterController = FindObjectOfType<SplitterController>();
        Debug.Log("SplitterController found: " + (splitterController != null));
    }

    private void Start()
    {
        nextTarget = FindPointOnNavMesh(transform.position, initialSearchRadius, minJumpDistance);
        Debug.Log("Next target found on NavMesh: " + nextTarget);
        MoveToPoint();
    }
    #endregion

    #region Public Functions
    // Publicly accessible functionality goes here, if any
    #endregion

    #region Private Functions
    private void MoveToPoint()
    {
        Debug.Log("Starting movement to point: " + nextTarget);
        StartCoroutine(MoveToTargetCoroutine(nextTarget));
    }

    private IEnumerator MoveToTargetCoroutine(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, target);
        Debug.Log("Calculated distance to target: " + distance);
        float travelTime = distance / speed; // Total travel time

        float elapsedTime = 0f; // Time that has passed

        while (true)
        {
            elapsedTime += Time.deltaTime;
            // Calculate the current proportion of the distance covered: 0 at start, 1 at target.
            float proportion = Mathf.Clamp01(elapsedTime / travelTime);

            // Calculate the new position.
            Vector3 horizontalPosition = Vector3.Lerp(startPosition, target, proportion);
            float verticalPosition = Mathf.Sin(proportion * Mathf.PI) * maxHeight;
            Vector3 newPosition = new Vector3(horizontalPosition.x, startPosition.y + verticalPosition, horizontalPosition.z);

            // Move to the new position.
            transform.position = newPosition;

            // Rotate towards the target
            Vector3 directionToTarget = (target - transform.position).normalized;
            directionToTarget.y = 0; // Keep the object upright
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // If the horizontal distance to the target is below the threshold, start lerping towards the target's y position
            float horizontalDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.x, target.z));
            if (horizontalDistance <= TargetThreshold)
            {
                Debug.Log("Start lerping towards the target's y position");
                while (Mathf.Abs(transform.position.y - target.y) > TargetThreshold)
                {
                    transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
                    yield return null;
                }
                break;
            }

            yield return null;
        }

        // Ensure the final position is exactly the target position.
        transform.position = target;
        Debug.Log("Reached target: " + transform.position);
        TriggerEndEvent();
    }

    void TriggerEndEvent()
    {
        splitterController.ObjectLanded();
    }

    private Vector3 FindPointOnNavMesh(Vector3 start, float searchRadius, float minDistance = 1f, float maxSearchRadius = 100f)
    {
        Vector3 point = Vector3.zero;
        bool pointFound = false;
        NavMeshHit hit;

        while (!pointFound && searchRadius <= maxSearchRadius)
        {
            // Calculate the direction towards the target
            Vector3 directionToTarget = (targetTransform.position - start).normalized;

            // Generate a random direction within the cone
            Vector3 randomDirection = Quaternion.Euler(0, Random.Range(-coneAngle, coneAngle), 0) * directionToTarget;

            // Scale the random direction by the minimum distance and the search radius to get two points
            Vector3 minPoint = start + randomDirection * minDistance;
            Vector3 maxPoint = start + randomDirection * searchRadius;

            bool foundMin = NavMesh.SamplePosition(minPoint, out NavMeshHit hitMin, minDistance, NavMesh.AllAreas);
            bool foundMax = NavMesh.SamplePosition(maxPoint, out NavMeshHit hitMax, searchRadius, NavMesh.AllAreas);

            if (foundMin && foundMax)
            {
                // Lerp between the two positions to find a point within the "donut-shaped" area
                float t = Random.value;
                point = Vector3.Lerp(hitMin.position, hitMax.position, t);
                pointFound = true;
                Debug.Log("Point found on NavMesh: " + point);
            }
            else
            {
                Debug.Log("Point not found on NavMesh, expanding search radius.");
                searchRadius *= 2;
            }
        }

        if (!pointFound)
        {
            Debug.LogError("Failed to find a valid point on the NavMesh within the maximum search radius!");
        }

        return point;
    }
    #endregion
}
