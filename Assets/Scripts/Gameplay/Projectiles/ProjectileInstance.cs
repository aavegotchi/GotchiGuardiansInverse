using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInstance : GameObjectInstance
{
    public enum ProjectileTypeID
    {
        INVALID,
        Projectile_BruteForce,
        Projectile_ROFL
    }

    public enum State
    {
        Inactive, // Initial state, should switch to Building automatically in controller
        Spawning, // Represents the projectile being built (Think reload)
        Idle, // Represents the Projectile being idle (loaded), ready to fire in this case
        Acting, // Represents the Projectile moving to its target or being activated
        Hit, // Represents the projectile resolving its hit
        Dead, // Represents the projectile being exhausted and should likely be cleaned up
    };

    #region Events
    public event Action<ProjectileInstance, State> OnStateChanged = delegate { };
    public event Action<ProjectileInstance, TowerController_Projectile> OnSetupForTower = delegate { };
    #endregion


    #region Variables
    //[Networked(OnChanged = nameof(OnSetState))]
    //public State CurrentState { get; set; } = State.Inactive;
    private State _currentState = State.Inactive;
    public State CurrentState
    {
        get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged.Invoke(this, _currentState);
            }
        }
    }

    [SerializeField]
    public ProjectileTypeID TypeID;

    // We don't rely on serialization here, its more for exposing to inspector for debugging purposes
    [Header("Dynamic Debug - null in prefab")]
    public TowerInstance_Projectile SpawningTowerInstance = null;
    public ProjectileController ProjectileController;
    public ProjectileManager ProjectileManager;
    public float SpawningProgress; // 0.0f = not spawned, 1.0f = fully spawned
    public float SpawnTime; // 1.0f = 1 second
    public int Damage;
    public float TravelSpeed;
    #endregion

    void Start()
    {
        gameObject.name = "Projectile Instance [" + TypeID.ToString() + "]: " + ID;
    }

    private void Update()
    {
        switch (CurrentState) 
        { 
            case State.Inactive:
                break;
            case State.Spawning:
                UpdateSpawning();
                break;
            case State.Idle:
                break;
            case State.Acting:
                break;
            case State.Hit:
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }

    #region public Functions
    // Called when the projectile is returned to the pool
    public void Cleanup()
    {
        SpawningTowerInstance = null;
        CurrentState = State.Inactive;
    }

    public void SetupFromTower(TowerInstance_Projectile tower)
    {
        SpawningTowerInstance = tower;
        SpawnTime = tower.TowerTemplateProjectile.ProjectileSpawnTime;
        Damage = tower.TowerTemplateProjectile.ProjectileDamage;
        TravelSpeed = tower.TowerTemplateProjectile.ProjectileSpeed;

        OnSetupForTower.Invoke(this, tower.gameObject.GetComponent<TowerController_Projectile>());
    }
    #endregion

    #region internal Functions
    private void UpdateSpawning()
    {
        SpawningProgress = Mathf.Clamp01(SpawningProgress + (Time.deltaTime / SpawnTime));

        if (SpawningProgress >= 1.0f)
        {
            CurrentState = State.Idle;
        }
    }
    #endregion
}
