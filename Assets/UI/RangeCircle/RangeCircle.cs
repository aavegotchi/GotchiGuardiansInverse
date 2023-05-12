using UnityEngine;

public class RangeCircle : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject rangeCircleOutline;
    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        ToggleActive(false);
    }

    #endregion

    #region Public Functions
    public void SetScale(float scale)
    {
        rangeCircleOutline.transform.localScale = new Vector3(scale, scale, 1);
    }

    public void ToggleActive(bool nextState)
    {
        rangeCircleOutline.SetActive(nextState);
    }
    #endregion
}