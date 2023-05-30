using UnityEngine;

public class FrogBounce_TestEnemyMovement : MonoBehaviour
{ 
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float speed = 1.0f;

    private float startTime;
    private float journeyLength;

    private void Start()
    {
        startPoint = transform.position;
        endPoint = startPoint + endPoint;

        // The time at which the animation started.
        startTime = Time.time;

        // The total distance between the start and end markers.
        journeyLength = Vector3.Distance(startPoint, endPoint);
    }

    private void Update()
    {
        // The distance moved equals elapsed time times speed.
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startPoint, endPoint, fractionOfJourney);
    }
}