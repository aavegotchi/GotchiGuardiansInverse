using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadialUI : MonoBehaviour
{
    [SerializeField]
    public List<RadialUIButton> ChoiceButtons = new List<RadialUIButton>();

    [SerializeField]
    List<Transform> ChoiceLocations = new List<Transform>();

    private GameObject shower;
    private Sequence activeSequence = null;

    public enum RadialState
    {
        Closed,
        Open,
        Closing,
        Opening
    }

    public delegate void RadialStateChanged(RadialUI radialUI, RadialState state);
    public event RadialStateChanged OnRadialStateChanged;

    public delegate void RadialButtonActivated(RadialUI radialUI, int index, RadialUIButton button);
    public event RadialButtonActivated OnRadialButtonActivated;

    private RadialState _state = RadialState.Closed;
    public RadialState State
    {
        get { return _state; }
        private set
        {
            if (_state != value)
            {
                _state = value;
                OnRadialStateChanged?.Invoke(this, _state);
            }
        }
    }

    public void ShowChoices(List<RadialUIButtonData> data, GameObject _shower)
    {
        shower = _shower;
        int _showAmount = data.Count > 5 ? 5 : data.Count;

        if (activeSequence != null)
        {
            activeSequence.Kill();
            activeSequence = null;
        }

        activeSequence = DOTween.Sequence();

        State = RadialState.Opening;

        int currentIndex = 0;
        foreach (RadialUIButton button in ChoiceButtons)
        {
            if (currentIndex < _showAmount)
            {
                button.gameObject.SetActive(true);
                button.Data = data[currentIndex];
                button.transform.position = transform.position;
                activeSequence.Insert(0.0f, button.transform.DOMove(ChoiceLocations[currentIndex].position, 0.6f));
                button.transform.localScale = Vector3.zero;
                activeSequence.Insert(0.0f, button.transform.DOScale(Vector3.one, 0.6f));
            }
            else 
            {
                if (button.isActiveAndEnabled)
                {
                    activeSequence.Insert(0.0f, button.transform.DOMove(transform.position, 0.6f));
                    activeSequence.Insert(0.0f, button.transform.DOScale(Vector3.zero, 0.6f));
                    activeSequence.AppendCallback(() => { button.gameObject.SetActive(false); });
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }

            ++currentIndex;
        }

        activeSequence.AppendCallback(() => { State = RadialState.Open; activeSequence = null; });
    }

    public void Hide()
    {
        if (activeSequence != null)
        {
            activeSequence.Kill();
            activeSequence = null;
        }

        State = RadialState.Closing;

        activeSequence = DOTween.Sequence();

        foreach (RadialUIButton button in ChoiceButtons)
        {
            if (button.isActiveAndEnabled)
            {
                activeSequence.Insert(0.0f, button.transform.DOMove(transform.position, 0.6f));
                activeSequence.Insert(0.0f, button.transform.DOScale(Vector3.zero, 0.6f));
            }
        }

        activeSequence.AppendCallback(() => { State = RadialState.Closed; activeSequence = null; });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                RadialUIButton hitButton = ColliderRedirector.GetFinalTarget<RadialUIButton>(hit.transform.gameObject);
                // A collider was hit by the ray. Check if it's a cube object.
                if (hitButton != null && ChoiceButtons.Contains(hitButton))
                {
                    OnRadialButtonActivated?.Invoke(this, ChoiceButtons.IndexOf(hitButton), hitButton);
                }
                else if (hit.transform.gameObject != shower)
                {
                    Hide();
                    // The ray hit a cube object. Do something...
                }
            }
            else
            {
                Hide();
            }
        }

    }
}