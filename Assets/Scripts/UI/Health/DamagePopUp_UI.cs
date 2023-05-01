using TMPro;
using UnityEngine;
using System.Collections;

public class DamagePopUp_UI : MonoBehaviour
{
    #region Fields
    [SerializeField] private TextMeshPro text = null;
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float movementDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private Vector2 distanceRange = new Vector2(0.5f, 1f);
    [SerializeField] private Vector2 angleRange = new Vector2(30f, 60f);
    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Color towerColor = Color.green;
    #endregion

    #region Private Variables
    private DamagePopUpManager damagePopUpManager;
    private Transform followTransform;
    private Transform textTransform;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        textTransform = text.transform;
    }

    private void Update()
    {
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
    }
    #endregion

    #region Public Functions
    public void SetDamagePopUpManager(DamagePopUpManager manager)
    {
        damagePopUpManager = manager;
    }

    public void SetFollowTransform(Transform newFollowTransform)
    {
        followTransform = newFollowTransform;
    }

    public void ShowAndHide(float damage, bool isEnemy)
    {
        SetDamageValue(damage);
        SetTextColor(isEnemy ? enemyColor : towerColor);
        ApplyRandomZOffset();
        StartCoroutine(ShowAndHideSequence());
    }

    private void ApplyRandomZOffset()
    {
        float randomZOffset = Random.Range(-0.01f, 0.01f);
        transform.localPosition += new Vector3(0, 0, randomZOffset);
    }
    #endregion

    #region Private Functions
    private void SetDamageValue(float damage)
    {
        text.text = damage.ToString();
    }

    private void SetTextColor(Color color)
    {
        text.color = color;
    }

    private void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f, fadeInDuration));
    }

    private void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f, fadeOutDuration));
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(start, end, t));
            yield return null;
        }
    }

    private void ScaleUp()
    {
        StartCoroutine(Scale(scaleFactor, fadeInDuration));
    }

    private void ScaleDown()
    {
        StartCoroutine(Scale(1f / scaleFactor, fadeOutDuration));
    }

    private IEnumerator Scale(float targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startScale = textTransform.localScale;
        Vector3 endScale = new Vector3(targetScale, targetScale, 1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            textTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }

    private void RandomUpwardMovement()
    {
        StartCoroutine(MoveUpwards());
    }

    private IEnumerator MoveUpwards()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = textTransform.localPosition;

        float xDirection = Random.Range(-1f, 1f);
        float distance = Random.Range(distanceRange.x, distanceRange.y);
        float angle = Mathf.Deg2Rad * Random.Range(angleRange.x, angleRange.y);
        Vector3 targetPosition = startPosition + new Vector3(Mathf.Cos(angle) * xDirection * distance, Mathf.Sin(angle) * distance, 0f);

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / movementDuration;
            textTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
    }

    private IEnumerator ShowAndHideSequence()
    {
        FadeIn();
        ScaleUp();
        yield return new WaitForSeconds(fadeInDuration);

        RandomUpwardMovement();
        yield return new WaitForSeconds(movementDuration);

        FadeOut();
        ScaleDown();
        followTransform = null;
        yield return new WaitForSeconds(fadeOutDuration);

        ResetAndReparent();
    }

    private void ResetAndReparent()
    {
        transform.SetParent(damagePopUpManager.transform, true);
        textTransform.localPosition = Vector3.zero;
        followTransform = null;
        gameObject.SetActive(false);
    }
    #endregion
}