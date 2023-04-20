using System.Collections;
using UnityEngine;

public class CanMovePopUp : MonoBehaviour
{
    #region Events
    #endregion

    #region Public Variables
    #endregion

    #region Fields

    [SerializeField] private Sprite canMoveHereSprite = null;
    [SerializeField] private Sprite cannotMoveHereSprite = null;
    [SerializeField] private Color canMoveHereColor = Color.green;
    [SerializeField] private Color cannotMoveHereColor = Color.red;
    [SerializeField] private float extraScale = 1.1f;
    // [SerializeField] private float finalScale = 1.1f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private float waitDuration = 1.0f;
    [SerializeField] private float closeAnimationDuration = 0.2f;
    #endregion

    #region Private Variables
    private SpriteRenderer spriteRenderer = null;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        spriteRenderer= GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Public Functions
    public void ShowCanMovePopUp(bool canMoveToTarget)
    {
        spriteRenderer.sprite = canMoveToTarget ? canMoveHereSprite : cannotMoveHereSprite;
        spriteRenderer.color = canMoveToTarget ? canMoveHereColor : cannotMoveHereColor;
        StartCoroutine(AnimatePopUp());
    }
    #endregion

    #region Private Functions
    private IEnumerator AnimatePopUp()
    {
        yield return Animate(0f, extraScale, animationDuration, 0f, 1f);
        yield return new WaitForSeconds(waitDuration);
        yield return Animate(extraScale, 0f, closeAnimationDuration, 1f, 0f);
        gameObject.SetActive(false);
    }

    private IEnumerator Animate(float startScale, float endScale, float duration, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Scale animation
            float currentScale = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);

            // Opacity animation
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            spriteRenderer.color = spriteColor;

            yield return null;
        }
    }
    #endregion
}
