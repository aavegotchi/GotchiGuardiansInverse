using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
    [SerializeField]
    protected ProjectileInstance ProjectileInstance;

    [SerializeField]
    private GameObject VisualRoot;

    // We don't rely on serialization here, its more for exposing to inspector for debugging purposes
    [Header("Dynamic Debug - null in prefab")]
    [SerializeField]
    private ProjectileVisual ProjectileVisual;

    protected Vector3 LaunchPosition;
    protected float LaunchY;
    protected Quaternion LaunchRotation;

    // We keep a last valid hit location because the target could be dead.
    // In that case projectile still needs a destination to hit even if it won't be updated anymore
    protected Vector3 HitLocation; 

    void Start()
    {
        if (ProjectileInstance == null)
        {
            Debug.LogError("ProjectileController has no ProjectileInstance assigned!");
            return;
        }

        ProjectileInstance.ProjectileController = this;

        ProjectileInstance.OnStateChanged += ProjectileInstance_OnStateChanged;

        ProjectileInstance.OnVisualsEnabledChanged += ProjectileInstance_OnVisualsEnabledChanged;
        UpdateVisuals();
    }

    private void Update()
    {
        switch (ProjectileInstance.CurrentState)
        {
            case ProjectileInstance.State.Spawning:
                UpdateSpawning();
                break;
            case ProjectileInstance.State.Idle:
                UpdateIdle();
                break;
            case ProjectileInstance.State.Acting:
                UpdateActing();
                break;
            case ProjectileInstance.State.Hit:
                UpdateHit();
                break;
            case ProjectileInstance.State.Dead:
                UpdateDead();
                break;
            default:
                break;
        }
    }

    public void Spawn()
    {
        if (ProjectileInstance == null)
        {
            Debug.LogError("ProjectileController has no ProjectileInstance assigned when trying to enter spawning state!");
            return;
        }

        if (ProjectileInstance.CurrentState != ProjectileInstance.State.Inactive)
        {
            Debug.LogWarning("ProjectileController[" + ProjectileInstance.ID +"] trying to enter spawning state when not in inactive state!");
        }

        ProjectileInstance.SpawningProgress = 0.0f;
        ProjectileInstance.EnterSpawning();
    }

    public virtual bool FireAtTarget(GameObject target)
    {
        if (ProjectileInstance.Target != null)
        {
            Debug.LogError("ProjectileController[" + ProjectileInstance.ID + "] trying to fire at target when already has a target!");
            return false;
        }

        if (ProjectileInstance.CurrentState != ProjectileInstance.State.Idle)
        {
            Debug.LogError("Tried to fire projectile when not in idle state!");
            return false;
        }

        ProjectileInstance.FireAtTarget(target);

        Debug.Log("ProjectileController[" + ProjectileInstance.ID + "] fired at target[" + target.name + "]");

        return true;
    }

    #region Internal Functions
    private void ProjectileInstance_OnStateChanged(ProjectileInstance projectileInstance, ProjectileInstance.State newState)
    {
        UpdateVisuals();

        switch (newState)
        {
            // Shouldn't happen, this is only an initialization state
            //case ProjectileInstance.State.Inactive:
            //    break;
            case ProjectileInstance.State.Spawning:
                HandleEnterSpawned();
                break;
            case ProjectileInstance.State.Idle:
                HandleEnterIdle();
                break;
            case ProjectileInstance.State.Acting:
                HandleEnterActing();
                break;
            case ProjectileInstance.State.Hit:
                HandleEnterHit();
                break;
            case ProjectileInstance.State.Dead:
                HandleEnterDead();
                break;
            default:
                break;
        }
    }

    protected virtual void HandleEnterSpawned()
    {

    }

    protected virtual void HandleEnterIdle()
    {

    }

    protected virtual void HandleEnterActing()
    {
        LaunchPosition = transform.position;
        LaunchRotation = transform.rotation;
        LaunchY = transform.forward.normalized.y;
    }

    protected virtual void HandleEnterHit()
    {
        //ImpactPool_FX.Instance.SpawnImpact(ImpactPool_FX.ImpactType.Arrow, transform.position, transform.rotation);
    }

    protected virtual void HandleEnterDead()
    {
        UpdateVisuals();
    }

    protected virtual void UpdateSpawning()
    {
        // Do nothing, derived classes can do more
    }

    protected virtual void UpdateIdle()
    {
        // Do nothing, derived classes can do more
    }

    protected virtual void UpdateActing()
    {
        if (ProjectileInstance.Target != null && ProjectileInstance.Target.gameObject.activeInHierarchy)
        {
            HitLocation = ProjectileInstance.Target.transform.position;
        }
    }

    protected virtual void UpdateHit()
    {
        // Do nothing, derived classes can do more
    }

    protected virtual void UpdateDead()
    {
        // Do nothing, derived classes can do more
    }

    private void ProjectileInstance_OnVisualsEnabledChanged(GameObjectInstance projectileInstance, bool isVisible)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (ProjectileInstance.CurrentState != ProjectileInstance.State.Dead && ProjectileInstance.CurrentState != ProjectileInstance.State.Inactive && 
            ProjectileInstance.VisualsEnabled && ProjectileVisual == null)
        {
            ProjectileVisual = ProjectileInstance.ProjectileManager.ClaimProjectileVisual(ProjectileInstance.TypeID);

            if (ProjectileVisual == null)
            {
                Debug.LogError("ProjectileController [" + ProjectileInstance.ID + "] failed to claim a ProjectileVisual!");
                return;
            }

            ProjectileVisual.gameObject.SetActive(true);
            ProjectileVisual.transform.SetParent(VisualRoot.transform, false);
            ProjectileVisual.AssignData(this, ProjectileInstance);
        }
        else if ((ProjectileInstance.CurrentState == ProjectileInstance.State.Dead || !ProjectileInstance.VisualsEnabled) && ProjectileVisual != null)
        {
            ProjectileInstance.ProjectileManager.FreeProjectileVisual(ProjectileVisual);
            ProjectileVisual = null;
        }
    }
    #endregion

}
