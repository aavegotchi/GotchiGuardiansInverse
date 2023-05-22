using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerVisual : MonoBehaviour
{
    [SerializeField]
    public TowerTemplate.TowerTypeID TypeID;
    [SerializeField]
    private RevealFXController RevealFXController;

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
    #endregion
}
