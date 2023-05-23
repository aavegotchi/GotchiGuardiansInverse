using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerVisual : MonoBehaviour
{
    [SerializeField]
    public TowerTemplate.TowerTypeID TypeID;
    [SerializeField]
    private RevealFXController RevealFXController;
    [SerializeField]
    private GameObject TurretRoot; // Optional root that will look at current target (if any)

    [Header("Dynamic Debug - null in prefab")]
    public TowerInstance Instance;
    public TowerController Controller;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurretLookat();
    }

    #region public interfaces
    public virtual void AssignData(TowerInstance instance, TowerController controller)
    {
        Instance = instance;
        Controller = controller;

        Instance.OnBuildingProgressChanged += Instance_OnBuildingProgressChanged;

        if (RevealFXController!= null )
        {
            if (Instance.CurrentState == TowerInstance.State.Building)
            {
                RevealFXController.Progress = Instance.BuildingProgress;
            }
            else if (Instance.CurrentState == TowerInstance.State.New)
            {
                RevealFXController.Progress = 0.0f;
            }
            else
            {
                RevealFXController.Progress = 1.0f;
            }
        }
    }

    public virtual void Cleanup()
    {
        Instance.OnBuildingProgressChanged -= Instance_OnBuildingProgressChanged;

        Instance = null;
        Controller = null;
    }
    #endregion

    #region Internal Funcs
    private void Instance_OnBuildingProgressChanged(TowerInstance towerInstance, float progress)
    {
        if (RevealFXController != null)
        {
            RevealFXController.Progress = progress;
        }
    }

    private void UpdateTurretLookat()
    {
        if (TurretRoot != null)
        {
            TowerController_Projectile towerController_Projectile = Controller as TowerController_Projectile;

            if (towerController_Projectile != null && towerController_Projectile.CurrentTarget != null)
            {
                Vector3 directionToTarget = towerController_Projectile.CurrentTarget.transform.position - TurretRoot.transform.position;
                directionToTarget.y = 0;  // This ensures the turret only rotates around the y-axis

                if (directionToTarget.sqrMagnitude > 0.0f)  // Only update the rotation if the direction is not zero
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                    // This smooths out the rotation over time (though still quickly) to prevent too aggressive snapping
                    TurretRoot.transform.rotation = Quaternion.RotateTowards(TurretRoot.transform.rotation, targetRotation, 300f * Time.deltaTime);
                }
            }
        }
    }


    #endregion
}
