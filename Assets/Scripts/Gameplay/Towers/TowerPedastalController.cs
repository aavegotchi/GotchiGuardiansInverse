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
        towerPedastalInstance.OnTowerInstanceChanged += TowerPedastalInstance_OnTowerInstanceChanged;
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

                if (hitObject == gameObject && towerPedastalInstance.TowerInstance == null)
                {
                    ShowRadialMenu();
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (towerPedastalInstance.TowerInstance == null)
        {
            placeholderMouseOverGO?.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (towerPedastalInstance.TowerInstance == null)
        {
            placeholderMouseOverGO?.SetActive(false);
        }
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
        }
    }

    private void OnRadialButtonActivated(RadialUI radialUI, int index, RadialUIButton button)
    {
        RadialUIButtonData_Tower towerButtonData = button.Data as RadialUIButtonData_Tower;

        if (towerButtonData != null)
        {
            towerPedastalInstance?.SpawnTower(towerButtonData.TypeID);
            activeRadial?.Hide();
        }
    }

    private void OnRadialStateChanged(RadialUI radialUI, RadialUI.RadialState state)
    {
        if (activeRadial != null && state == RadialUI.RadialState.Closed)
        {
            Destroy(activeRadial.gameObject);
            activeRadial = null;
        }
    }

    private void TowerPedastalInstance_OnTowerInstanceChanged(TowerPedastalInstance pedastal, TowerInstance newInstance)
    {
        if (newInstance != null)
        {
            placeholderMouseOverGO?.SetActive(false);
        }
    }
}
