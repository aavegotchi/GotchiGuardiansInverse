using Fusion;
using PhaseManager.Model;
using System;
using UnityEngine;
using static TowerInstance;

public class TowerInstance : GameObjectInstance
{
    public enum State
    {
        New, // Initial state, should switch to Building automatically in controller
        Building, // Represents the tower being built
        Idle, // Represents the tower being idle, likely searching for its victim. Purely passive towers remain in this state
        Acting, // Represents the tower doing logic to launch its attack
        Cooldown, // Represents the tower waiting for its cooldown to finish before returning to idle (after attacking / using ability)
        Upgrading, // Represents an upgrade in progress
    };

    #region Events
    public event Action<TowerInstance, State> OnStateChanged = delegate { };
    public event Action<TowerInstance, float> OnBuildingProgressChanged = delegate { };
    #endregion

    #region Variables
    public TowerController Controller { get; set; }

    //[Networked(OnChanged = nameof(OnSetState))]
    //public State CurrentState { get; set; } = State.New;
    private State _currentState = State.New;
    public State CurrentState
    {
        get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged_Internal();
            }
        }
    }

    private float BuildingProgress = 0.0f;

    //[Header("Dynamic Debug - null in prefab")]
    public TowerTemplate Template { get; set; } = null;
    public TowerPedastalInstance Pedastal { get; set; } = null;
    #endregion

    #region functions
    protected virtual void Start()
    {
        gameObject.name = "Tower Instance [" + Template.name + "]: " + ID;
    }

    //public override void FixedUpdateNetwork()
    protected virtual void Update()
    {
        if (CurrentState == State.Building)
        {
            UpdateBuilding();
        }
    }
    #endregion

    public void StartBuilding()
    {
        if (CurrentState != State.New)
        {
            Debug.LogError("Tower Instance " + ID + " tried to start building but was not in the New state!");
            return;
        }

        BuildingProgress = 0.0f;
        CurrentState = State.Building;
    }

    private void UpdateBuilding()
    {
        float newProgress = BuildingProgress + (Time.deltaTime / Template.buildTime);
        SetBuildingProgress(newProgress);

        if (BuildingProgress == 1.0f)
        {
            FinishBuilding();
        }
    }

    private void FinishBuilding()
    {
        CurrentState = State.Idle;
    }

    private void SetBuildingProgress(float progress)
    {
        float _validProgress = Mathf.Clamp(progress, 0.0f, 1.0f);
        if (BuildingProgress != _validProgress)
        {
            BuildingProgress = _validProgress;
            OnBuildingProgressChanged(this, BuildingProgress);
        }
    }

    #region network functions

    private static void OnSetState(Changed<TowerInstance> changed)
    {
        changed.Behaviour.OnStateChanged_Internal();
    }

    private void OnStateChanged_Internal()
    {
        OnStateChanged?.Invoke(this, CurrentState);

        if (CurrentState == State.Idle)
        {
            SetBuildingProgress(1.0f);
        }
        else if (CurrentState == State.Building)
        {
            SetBuildingProgress(0.0f);
        }
    }
    #endregion
}
