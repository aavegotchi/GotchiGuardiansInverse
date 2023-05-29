using Fusion;
using Gotchi.Lickquidator.Manager;
using Gotchi.Lickquidator.Presenter;
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
    public event Action<ProjectileInstance, GameObject> OnTargetChanged = delegate { };
    #endregion

    [SerializeField]
    public float HitDuration = 1.0f;

    private float _hitDurationTimer = 0.0f;


    #region Variables
    //[Networked(OnChanged = nameof(OnSetState))]
    //public State CurrentState { get; set; } = State.Inactive;
    private State _currentState = State.Inactive;
    public State CurrentState
    {
        get { return _currentState; }
        protected set
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
    public float ActingTime { get; private set; } = 0.0f;

    public GameObject Target { get; private set; } = null;
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
                UpdateActing();
                break;
            case State.Hit:
                UpdateHit();
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
        Target = null;
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

    public void FireAtTarget(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("ProjectileInstance[" + ID + "]: Cannot fire at null target!");
            return;
        }
        
        if (Target != null) 
        {
            Debug.LogError("ProjectileInstance[" + ID + "]: Cannot fire at target when projectile already has a target");
            return;
        }

        Target = target;
        OnTargetChanged.Invoke(this, target);
        EnterActing();
    }
    #endregion

    #region internal Functions
    public void EnterIdle()
    {
        CurrentState = State.Idle;
    }

    public void EnterSpawning()
    {
        CurrentState = State.Spawning;
    }

    private void UpdateSpawning()
    {
        SpawningProgress = Mathf.Clamp01(SpawningProgress + (Time.deltaTime / SpawnTime));

        if (SpawningProgress >= 1.0f)
        {
            FinishSpawning();
        }
    }

    private void FinishSpawning()
    {
        CurrentState = State.Idle;
    }

    public void EnterActing()
    {
        if (CurrentState != State.Idle) 
        {
            Debug.LogError("ProjectileInstance: Cannot enter Acting state from state: " + CurrentState.ToString());
            return;
        }

        ActingTime = 0.0f;
        CurrentState = State.Acting;
    }

    private void UpdateActing()
    {
        ActingTime += Time.deltaTime;
    }

    public void EnterHit()
    {
        _hitDurationTimer = 0.0f;
        CurrentState = State.Hit;

        if (Target != null && Target.activeInHierarchy && Target.tag == "Enemy")
        {
            LickquidatorPresenter enemy = LickquidatorManager.Instance.GetByObject(Target);

            if (enemy != null)
            {
                enemy.Damage(Damage);
            }
        }
    }

    protected virtual void UpdateHit()
    {
        _hitDurationTimer += Time.deltaTime;

        if (_hitDurationTimer >= HitDuration)
        {
            CurrentState = State.Dead;
        }
    }
    #endregion
}
