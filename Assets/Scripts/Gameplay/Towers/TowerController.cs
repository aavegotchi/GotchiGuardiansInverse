using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using static UnityEngine.CullingGroup;

public class TowerController : NetworkBehaviour
{
    #region Fields
    [SerializeField]
    public TowerInstance TowerInstance;
    [SerializeField]
    private RevealFXController RevealFXController;
    #endregion

    #region Unity Funcs
    // Start is called before the first frame update
    protected virtual void Start()
    {
        TowerInstance.OnStateChanged += TowerInstance_OnStateChanged;
        TowerInstance.OnBuildingProgressChanged += TowerInstance_OnBuildingProgressChanged;
        Spawned();
    }

    // Update is called once per frame
    //public override void FixedUpdateNetwork()
    protected virtual void Update()
    {
        UpdateStates();
        
    }
    #endregion

    #region Internal Funcs
    private void TowerInstance_OnBuildingProgressChanged(TowerInstance instance, float buildProgress)
    {
        if (RevealFXController != null)
        {
            RevealFXController.Progress = buildProgress;
        }
    }

    private void TowerInstance_OnStateChanged(TowerInstance instance, TowerInstance.State state)
    {
        switch (state)
        {
            //case TowerInstance.State.New:
            //    break;
            case TowerInstance.State.Building:
                HandleEnter_BuildingState();
                break;
            case TowerInstance.State.Idle:
                HandleEnter_IdleState();
                break;
            case TowerInstance.State.Acting:
                HandleEnter_ActingState();
                break;
            case TowerInstance.State.Cooldown:
                HandleEnter_CooldownState();
                break;
            case TowerInstance.State.Upgrading:
                HandleEnter_UpgradingState();
                break;
            default:
                break;
        }
    }

    private void UpdateStates()
    {
        bool stateUnchanged = true;
        switch (TowerInstance.CurrentState)
        {
            case TowerInstance.State.New:
                stateUnchanged = UpdateNewState();
                break;
            case TowerInstance.State.Building:
                stateUnchanged = UpdateBuildingState();
                break;
            case TowerInstance.State.Idle:
                stateUnchanged = UpdateIdleState();
                break;
            case TowerInstance.State.Acting:
                stateUnchanged = UpdateActingState();
                break;
            case TowerInstance.State.Cooldown:
                stateUnchanged = UpdateCooldown();
                break;
            case TowerInstance.State.Upgrading:
                stateUnchanged = UpdateUpgrading();
                break;
            default:
                break;
        }

        if (!stateUnchanged) 
        {
            UpdateStates();
        }
    }

    // Return value is to indicate to derived classes if they should continue to update
    protected virtual bool UpdateNewState()
    {
        TowerInstance.StartBuilding();
        return true;
    }

    protected virtual void HandleEnter_BuildingState()
    {

    }

    protected virtual bool UpdateBuildingState()
    {
        return true;
    }

    protected virtual void HandleEnter_IdleState()
    {
    }

    protected virtual bool UpdateIdleState()
    {
        return true;
    }

    protected virtual void HandleEnter_ActingState()
    {
    }

    protected virtual bool UpdateActingState()
    {
        return true;
    }

    protected virtual void HandleEnter_CooldownState()
    {
    }

    protected virtual bool UpdateCooldown()
    {
        return true;
    }

    protected virtual void HandleEnter_UpgradingState()
    {
    }

    protected virtual bool UpdateUpgrading()
    {
        return true;
    }
    #endregion
}
