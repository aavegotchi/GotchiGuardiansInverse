using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController_Simple : ProjectileController
{
    protected override void UpdateActing()
    {
        base.UpdateActing();

        float currentRemainingDistance = Vector3.Distance(transform.position, HitLocation);
        float moveDistance = ProjectileInstance.TravelSpeed * Time.deltaTime;

        // If the projectile would move past the target, snap it to the target and trigger the hit event.
        if (moveDistance >= currentRemainingDistance)
        {
            transform.position = HitLocation;
            ProjectileInstance.EnterHit();
            return;
        }

        float updatedDistanceFromLaunch = Vector3.Distance(LaunchPosition, HitLocation);

        Vector3 directionToTarget = (HitLocation - transform.position).normalized;

        // Calculate the lerp factor based on the elapsed time
        float lerpFactor = ProjectileInstance.ActingTime / 0.5f; // Adjust as needed

        // Gradually increase the lerp factor to make the turn more aggressive over time
        lerpFactor = Mathf.Pow(lerpFactor, 2);

        // Cap the lerp factor to 1.0f
        lerpFactor = Mathf.Clamp01(lerpFactor);

        // Lerp between the forward direction and directionToTarget based on the lerp factor
        Vector3 lerpedDirection = Vector3.Lerp(transform.forward, directionToTarget, lerpFactor);

        // Move the projectile towards the lerped direction
        Vector3 movement = lerpedDirection * moveDistance;
        transform.position += movement;

        transform.LookAt(transform.position + lerpedDirection);
    }

}
