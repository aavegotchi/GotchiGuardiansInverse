using System.Collections;
using UnityEngine;

public class DeactivateParticleSystemAfterPlayback : MonoBehaviour
{
    #region Fields
    [SerializeField] private bool isLoop = false;
    #endregion

    #region Private Variables
    private ParticleSystem particles = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        StartCoroutine(deactivateObjectAfterParticleSystemPlayback());
    }
    #endregion

    #region Private Functions
    private IEnumerator deactivateObjectAfterParticleSystemPlayback()
    {
        do
        {
            particles.Play();
            yield return new WaitForSeconds(particles.main.duration);
        } while (isLoop);

        gameObject.SetActive(false);
    }
    #endregion
}
