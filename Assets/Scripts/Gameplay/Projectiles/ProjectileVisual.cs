using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    [SerializeField]
    public ProjectileInstance.ProjectileTypeID TypeID;

    // We don't rely on serialization here, its more for exposing to inspector for debugging purposes
    [Header("Dynamic Debug - null in prefab")]
    public ProjectileManager ProjectileManager;
    public ProjectileController Controller;
    public ProjectileInstance Instance;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void Cleanup()
    {
        Instance = null;
        Controller = null;
    }

    public virtual bool AssignData(ProjectileController projectileController, ProjectileInstance instance)
    {
        Controller = projectileController;
        Instance = instance;

        if (instance == null || projectileController == null)
        {
            Debug.LogError("Projectile Visual AssignData called with null instance or controller");
            return false;
        }

        return true;
    }
}
