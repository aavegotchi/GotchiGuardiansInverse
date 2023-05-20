using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class TowerController : NetworkBehaviour
{
    [SerializeField]
    public TowerInstance TowerInstance;
    [SerializeField]
    private RevealFXController RevealFXController;

    // Start is called before the first frame update
    void Start()
    {
        TowerInstance.OnStateChanged += TowerInstance_OnStateChanged;
        TowerInstance.OnBuildingProgressChanged += TowerInstance_OnBuildingProgressChanged;
        Spawned();
    }

    // Update is called once per frame
    //public override void FixedUpdateNetwork()
    public void Update()
    {
        if (TowerInstance.CurrentState == TowerInstance.State.New)
        {
            TowerInstance.StartBuilding();
        }
    }

    private void TowerInstance_OnBuildingProgressChanged(TowerInstance instance, float buildProgress)
    {
        if (RevealFXController != null)
        {
            RevealFXController.progress = buildProgress;
        }
    }

    private void TowerInstance_OnStateChanged(TowerInstance instance, TowerInstance.State state)
    {
        
    }
}
