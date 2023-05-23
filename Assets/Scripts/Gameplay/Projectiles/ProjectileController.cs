using Fusion;
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

    private GameObject Target;

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
        ProjectileInstance.CurrentState = ProjectileInstance.State.Spawning;
    }

    public virtual bool FireAtTarget(GameObject target)
    {
        if (Target != null)
        {
            Debug.LogError("ProjectileController[" + ProjectileInstance.ID + "] trying to fire at target when already has a target!");
            return false;
        }

        if (ProjectileInstance.CurrentState != ProjectileInstance.State.Idle)
        {
            Debug.LogError("Tried to fire projectile when not in idle state!");
            return false;
        }

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
                HandleSpawned();
                break;
            case ProjectileInstance.State.Idle:
                HandleIdle();
                break;
            case ProjectileInstance.State.Acting:
                HandleActing();
                break;
            case ProjectileInstance.State.Hit:
                Hit();
                break;
            case ProjectileInstance.State.Dead:
                HandleDead();
                break;
            default:
                break;
        }
    }

    protected virtual void HandleSpawned()
    {

    }

    protected virtual void HandleIdle()
    {

    }

    protected virtual void HandleActing()
    {

    }

    protected virtual void Hit()
    {

    }

    protected virtual void HandleDead()
    {
        UpdateVisuals();
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
