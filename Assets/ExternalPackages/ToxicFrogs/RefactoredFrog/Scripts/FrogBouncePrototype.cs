using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FrogBouncePrototype : MonoBehaviour
{
    [SerializeField] private int bouncesRemaining = 2;
    [SerializeField] private float maxBounceDistance = 10f;
    [SerializeField] private float flightSpeed = 2f;
    [SerializeField] private float rotationSpeed = 50f; // New rotation speed variable
    [SerializeField] private float pauseDuration = 1f; // Pause duration variable

    [SerializeField] private Transform nextTarget = null;
    [SerializeField] private List<GameObject> bouncedObjects = new List<GameObject>();

    private bool isMoving = false;

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
        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        // Move towards the target
        float step = flightSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextTarget.position, step);

        // Rotate towards the target
        Vector3 targetDirection = nextTarget.position - transform.position;
        float singleStep = rotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        // Check if reached the target
        if (Vector3.Distance(transform.position, nextTarget.position) < 0.001f)
        {
            StartCoroutine(PauseAtTarget());
        }
    }

    private IEnumerator PauseAtTarget()
    {
        isMoving = false;
        yield return new WaitForSeconds(pauseDuration);
        DecideNextTarget();
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
