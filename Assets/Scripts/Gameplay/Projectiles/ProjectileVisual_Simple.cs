using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVisual_Simple : ProjectileVisual
{
    //public enum State
    //{
    //    Inactive, // Initial state, should switch to Building automatically in controller
    //    Spawning, // Represents the projectile being built (Think reload)
    //    Idle, // Represents the Projectile being idle (loaded), ready to fire in this case
    //    Acting, // Represents the Projectile moving to its target or being activated
    //    Hit, // Represents the projectile resolving its hit
    //    Dead, // Represents the projectile being exhausted and should likely be cleaned up
    //};

    #region fields
    [Header("FX Configurations")]
    [SerializeField]
    private RevealFXController RevealFXController;

    [SerializeField]
    private GameObject SpawningFXRoot = null;

    [SerializeField]
    private GameObject IdleFXRoot = null;

    [SerializeField]
    private GameObject ActingFXRoot = null;

    [SerializeField]
    private GameObject HitFXRoot = null;

    private List<GameObject> FXRoots = new List<GameObject>();
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        if (SpawningFXRoot != null)
        {
            FXRoots.Add(SpawningFXRoot);
        }

        if (IdleFXRoot != null)
        {
            FXRoots.Add(IdleFXRoot);
        }

        if (ActingFXRoot != null)
        {
            FXRoots.Add(ActingFXRoot);
        }

        if (HitFXRoot != null)
        {
            FXRoots.Add(HitFXRoot);
        }

        DisabledAllFXRoots();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Instance != null)
        {
            if (RevealFXController != null && Instance.CurrentState == ProjectileInstance.State.Spawning)
            {
                RevealFXController.Progress = Instance.SpawningProgress;
            }
        }
    }

    #region public functions
    public override bool AssignData(ProjectileController projectileController, ProjectileInstance instance)
    {
        if (!base.AssignData(projectileController, instance))
        {
            return false;
        }

        instance.OnStateChanged += Instance_OnStateChanged;

        UpdateBasedOnState();

        return true;
    }

    private void Instance_OnStateChanged(ProjectileInstance arg1, ProjectileInstance.State arg2)
    {
        UpdateBasedOnState();
    }
    #endregion

    #region Internal functions
    private void UpdateBasedOnState()
    {
        DisabledAllFXRoots();

        switch (Instance.CurrentState)
        {
            case ProjectileInstance.State.Inactive:
                RevealFXController.Progress = 0.0f;
                break;
            case ProjectileInstance.State.Spawning:
                if (SpawningFXRoot != null)
                {
                    SpawningFXRoot.SetActive(true);
                }
                RevealFXController.Progress = Instance.SpawningProgress;
                break;
            case ProjectileInstance.State.Idle:
                if (IdleFXRoot != null)
                {
                    IdleFXRoot.SetActive(true);
                }
                RevealFXController.Progress = 1.0f;
                break;
            case ProjectileInstance.State.Acting:
                if (ActingFXRoot != null)
                {
                    ActingFXRoot.SetActive(true);
                }
                RevealFXController.Progress = 1.0f;
                break;
            case ProjectileInstance.State.Hit:
                if (HitFXRoot != null)
                {
                    HitFXRoot.SetActive(true);
                }
                RevealFXController.Progress = 1.0f;
                break;
            case ProjectileInstance.State.Dead:
                RevealFXController.Progress = 0.0f;
                break;
            default:
                break;
        }
    }

    private void DisabledAllFXRoots()
    {
        foreach (var fxRoot in FXRoots)
        {
            fxRoot.SetActive(false);
        }
    }
    #endregion
}
