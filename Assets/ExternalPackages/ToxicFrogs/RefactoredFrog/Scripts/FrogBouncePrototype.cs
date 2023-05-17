using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FrogBouncePrototype : MonoBehaviour
{
    [SerializeField] private int bouncesRemaining = 2;
    [SerializeField] private float maxBounceDistance = 10f;
    [SerializeField] private float flightSpeed = 2f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private float standbyDuration = 2f;  // delay between targets
    [SerializeField] private float maxHeight = 5f;  // max height in units

    [SerializeField] private Vector3 targetOffset; // target offset

    private Animator animator; // animator

    // Serialized for Debugging
    [SerializeField] private bool shouldLookAtNextTarget = false;
    [SerializeField] private bool isPausing = false;
    [SerializeField] private bool hasReachedTarget = false;
    [SerializeField] private bool isLockedToTarget = false;


    private Vector3 targetPositionWithOffset; // holds the target position plus the offset
    private static readonly int JumpStartHash = Animator.StringToHash("jumpStart");
    private static readonly int JumpFinishHash = Animator.StringToHash("jumpFinish");



    [SerializeField] private Transform nextTarget = null;
    [SerializeField] private Transform futureTarget = null; // for storing the future target
    private List<GameObject> bouncedObjects = new List<GameObject>();

    private bool isMoving = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (nextTarget != null)
        {
            if (isMoving)
            {
                MoveTowardsTarget();
            }
            else if (shouldLookAtNextTarget)
            {
                LookAtTarget(futureTarget);
            }
            else if (isLockedToTarget)
            {
                transform.position = nextTarget.position + targetOffset;
                if (futureTarget != null)
                {
                    LookAtTarget(futureTarget);
                }
            }
        }
    }


    public void SetNextTarget(Transform target)
    {
        nextTarget = target;
        targetPositionWithOffset = nextTarget.position + targetOffset; // calculate and store the target position plus the offset
        animator.SetTrigger(JumpStartHash); // start jump animation
        isMoving = true; // start moving
        shouldLookAtNextTarget = false; // stop looking at the next target
    }

    private void MoveTowardsTarget()
    {
        // Calculate horizontal distance to target
        Vector3 horizontalPosition = new Vector3(transform.position.x, targetPositionWithOffset.y, transform.position.z);
        float horizontalDistance = Vector3.Distance(horizontalPosition, targetPositionWithOffset);

        // Move towards the target
        float step = flightSpeed * Time.deltaTime;
        Vector3 targetPositionWithVerticalOffset = new Vector3(targetPositionWithOffset.x, targetPositionWithOffset.y, targetPositionWithOffset.z); // Copy the target position with the offset

        if (horizontalDistance > 0.1f) // Still moving towards the target
        {
            // Add vertical sinusoidal offset
            float verticalOffset = Mathf.Sin(horizontalDistance * Mathf.PI / maxBounceDistance) * maxHeight; // Use maxHeight instead of flightSpeed
            targetPositionWithVerticalOffset.y += verticalOffset;
        }


        transform.position = Vector3.MoveTowards(transform.position, targetPositionWithVerticalOffset, step);

        // Rotate towards the target
        Vector3 targetDirection = targetPositionWithOffset - transform.position; // Use the actual target position with the offset here
        float singleStep = rotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        // Check if reached the target
        if (Vector3.Distance(transform.position, targetPositionWithOffset) < 0.001f)
        {
            if (!hasReachedTarget && !isPausing)
            {
                animator.SetTrigger(JumpFinishHash); // finish jump animation
                StartCoroutine(PauseAtTarget());
                hasReachedTarget = true;

                // Lock the frog to the target
                isLockedToTarget = true;
            }
        }
        else
        {
            hasReachedTarget = false; // reset the flag if the frog has moved away from the target
            isLockedToTarget = false; // unlock the frog from the target
        }
    }

    private IEnumerator PauseAtTarget()
    {
        // Start of the coroutine: Set isPausing to true and isMoving to false
        isPausing = true;
        isMoving = false;

        // Pause at the target for the duration of pauseDuration
        yield return new WaitForSeconds(pauseDuration);

        // Decide the next target and set shouldLookAtNextTarget to true
        DecideNextTarget();
        shouldLookAtNextTarget = true;

        // If there's a future target, immediately call LookAtNextTarget
        if (futureTarget != null)
        {
            LookAtTarget(futureTarget);
        }

        // Wait for standbyDuration before moving to the next target
        yield return new WaitForSeconds(standbyDuration);

        // If shouldLookAtNextTarget is true, set it to false
        if (shouldLookAtNextTarget)
        {
            shouldLookAtNextTarget = false;
        }

        // If a future target exists, unlock the frog from the target and set the next target
        if (futureTarget != null)
        {
            isLockedToTarget = false;
            SetNextTarget(futureTarget);
        }

        // Clear future target and set hasReachedTarget and isPausing to false
        futureTarget = null;
        hasReachedTarget = false;
        isPausing = false;
    }


    private void LookAtTarget(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0;  // zero out the y-component of the direction vector to only rotate around Y-axis
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void DecideNextTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, maxBounceDistance, LayerMask.GetMask("Lickquidator"));

        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider enemyCollider in enemiesInRange)
        {
            GameObject enemy = enemyCollider.gameObject;
            if (enemy.tag != "Enemy" || bouncedObjects.Contains(enemy)) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            bouncedObjects.Add(closestEnemy);
            futureTarget = closestEnemy.transform; // store the future target instead of setting it immediately
        }
        else
        {
            DisableObject();
        }
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
        bouncedObjects.Clear();
    }
}
