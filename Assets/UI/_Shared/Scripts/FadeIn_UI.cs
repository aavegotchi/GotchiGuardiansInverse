using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeIn_UI : MonoBehaviour
{
    #region Fields
    [SerializeField] private float fadeDuration = 1f;
    #endregion

    #region Private Variables
    private Image fadeInImage;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        fadeInImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Private Functions
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeFromOpaqueToTransparent(fadeDuration, () =>
        {
            gameObject.SetActive(false);
        }));
    }

    private IEnumerator FadeFromOpaqueToTransparent(float duration, System.Action onComplete)
    {
        Color startColor = fadeInImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            fadeInImage.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        fadeInImage.color = endColor;

        onComplete?.Invoke();
    }
    #endregion
}