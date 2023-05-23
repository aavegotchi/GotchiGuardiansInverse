using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterController : MonoBehaviour
{
    public GameObject splitter1;
    public GameObject splitter2;

    public GameObject splitterJumpTest;
    private Vector3 landingPosition;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("TriggerSplitterMockDeath", 2);
    }

    void TriggerSplitterMockDeath()
    {
        splitter1.gameObject.SetActive(false);
        splitterJumpTest.gameObject.SetActive(true);
    }

    public void ObjectLanded()
    {
        // Store the landing position of the first splitter
        landingPosition = splitterJumpTest.transform.position;
        // Store the rotation of the first splitter
        Quaternion landingRotation = splitterJumpTest.transform.rotation;

        splitterJumpTest.gameObject.SetActive(false);

        // Set the position of the second splitter to the landing position
        splitter2.transform.position = landingPosition;
        // Set the rotation of the second splitter to the landing rotation
        splitter2.transform.rotation = landingRotation;

        // Enable the second splitter
        splitter2.SetActive(true);
    }
}