using System.Collections;
using UnityEngine;

public class FrogBounceController : MonoBehaviour
{
    [SerializeField] FrogBouncePrototype frogBouncePrototype;
    [SerializeField] Transform firstTarget;

    private void Start()
    {
        StartCoroutine(AssignFrogPrototypeTarget());
    }

    IEnumerator AssignFrogPrototypeTarget()
    {
        yield return new WaitForSeconds(1f);
        frogBouncePrototype.SetNextTarget(firstTarget);
    }
}
