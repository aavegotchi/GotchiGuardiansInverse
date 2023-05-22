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

        splitterJumpTest.gameObject.SetActive(false);

        // Set the position of the second splitter to the landing position
        splitter2.transform.position = landingPosition;

        // Enable the second splitter
        splitter2.SetActive(true);
    }
}