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

    [SerializeField] private Vector3 targetOffset; // target offset

    private Animator animator; // animator
    private Vector3 targetPositionWithOffset; // holds the target position plus the offset
    private static readonly int JumpStartHash = Animator.StringToHash("jumpStart");
    private static readonly int JumpFinishHash = Animator.StringToHash("jumpFinish");

    [SerializeField] private Transform nextTarget = null;
    [SerializeField] private List<GameObject> bouncedObjects = new List<GameObject>();

    private bool isMoving = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (nextTarget != null && isMoving)
        {
            MoveTowardsTarget();
        }
    }

    public void SetNextTarget(Transform target)
    {
        nextTarget = target;
        targetPositionWithOffset = nextTarget.position + targetOffset; // calculate and store the target position plus the offset
        animator.SetTrigger(JumpStartHash); // start jump animation
        if (!isMoving) // only start moving if currently not moving
        {
            isMoving = true;
        }
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
            float verticalOffset = Mathf.Sin(horizontalDistance * Mathf.PI / maxBounceDistance) * flightSpeed;
            targetPositionWithVerticalOffset.y += verticalOffset;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPositionWithVerticalOffset, step);

        // Rotate towards the target
        Vector3 targetDirection = targetPositionWithOffset - transform.position; // Use the actual target position with the offset here
        float singleStep = rotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        // Check if reached the target
        if (Vector3.Distance(transform.position, targetPositionWithOffset) < 0.001f) // Use the actual target position with the offset here
        {
            animator.SetTrigger(JumpFinishHash); // finish jump animation
            StartCoroutine(PauseAtTarget());
        }
    }

    private IEnumerator PauseAtTarget()
    {
        isMoving = false;
        yield return new WaitForSeconds(pauseDuration);
        DecideNextTarget();
        LookAtNextTarget();  // look at the next target
        yield return new WaitForSeconds(standbyDuration);  // wait for standbyDuration before moving to the next target
        isMoving = false;  // set isMoving to false at the end
    }


    private void LookAtNextTarget()
    {
        Vector3 targetDirection = nextTarget.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetDirection);
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
            SetNextTarget(closestEnemy.transform);
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
