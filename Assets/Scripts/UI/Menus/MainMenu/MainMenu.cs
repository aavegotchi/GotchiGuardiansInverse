using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas = null;
    [SerializeField] private Animator mainMenuCanvasAnimator = null;
    [SerializeField] private string mainMenuCanvasAnimatorCloseTrigger = "Close";

    public void StartFreePlay()
    {
        EventBus.MenuEvents.MenuItemSelected();
        mainMenuCanvasAnimator.SetTrigger(mainMenuCanvasAnimatorCloseTrigger);
        StartCoroutine(waitAndStart());
    }

    private IEnumerator waitAndStart()
    {
        yield return new WaitForSeconds(1.1f);
        mainMenuCanvas.SetActive(false);
        PhaseManager.Instance.StartNextPhase();
    }
}
