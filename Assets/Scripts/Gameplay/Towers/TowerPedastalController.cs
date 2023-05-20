using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using static RadialUIButton;
using System;

public class TowerPedastalController : NetworkBehaviour
{
    [SerializeField]
    public TowerPedastalInstance towerPedastalInstance;

    [SerializeField]
    public GameObject radialMenuPrefab;

    [SerializeField]
    public GameObject radialMenuRoot;

    [SerializeField]
    public GameObject placeholderMouseOverGO;

    private RadialUI activeRadial = null;

    // Start is called before the first frame update
    void Start()
    {
        
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
                GameObject hitObject = ColliderRedirector.GetFinalTarget(hit.transform.gameObject);

                // A collider was hit by the ray. Check if it's a cube object.
                if (hitObject == gameObject) // replace "Cube" with the appropriate tag
                {
                    ShowRadialMenu();
                    // The ray hit a cube object. Do something...
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        placeholderMouseOverGO?.SetActive(true);
    }

    private void OnMouseExit()
    {
        placeholderMouseOverGO?.SetActive(false);
    }

    private void ShowRadialMenu()
    {
        if (activeRadial == null)
        {
            activeRadial = Instantiate(radialMenuPrefab, radialMenuRoot.transform).GetComponent<RadialUI>();
            activeRadial.OnRadialStateChanged += OnRadialStateChanged;
            activeRadial.OnRadialButtonActivated += OnRadialButtonActivated;
            activeRadial.ShowChoices(TowerManager.Singleton.GetTowerBuildOptions(), gameObject);
        }
        else
        {
            activeRadial.Hide();
            //activeRadial.ShowChoices(GameplayData.Singleton.towerTemplates.Count, gameObject);
        }
    }

    private void OnRadialButtonActivated(RadialUI radialUI, int index, RadialUIButton button)
    {
    }

    private void OnRadialStateChanged(RadialUI radialUI, RadialUI.RadialState state)
    {
        if (activeRadial != null && state == RadialUI.RadialState.Closed)
        {
            Destroy(activeRadial.gameObject);
            activeRadial = null;
        }
    }
}
