using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RadialUIButton : MonoBehaviour
{
    public delegate void RadialBtnActivated(RadialUIButton button);
    public event RadialBtnActivated OnRadialBtnActivated;

    [SerializeField]
    private GameObject iconHolder;

    private RadialUIButtonData _data = null;
    public RadialUIButtonData Data
    {
        get { return _data; }
        set 
        { 
            _data = value; 

            if (iconHolder != null)
            {
                iconHolder.GetComponent<MeshRenderer>().material.SetTexture("_Texture", _data.ButtonImage);
            }
        }
    }

    private void OnMouseEnter()
    {
        transform.localScale = Vector3.one * 1.2f;
    }

    private void OnMouseExit()
    {
        transform.localScale = Vector3.one;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
