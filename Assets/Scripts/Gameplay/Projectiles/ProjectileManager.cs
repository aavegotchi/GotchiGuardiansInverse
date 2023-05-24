using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

[Serializable] public class ProjectileInstancePrefabDictionary : SerializableDictionary<ProjectileInstance.ProjectileTypeID, GameObject> { }

[Serializable] public class ProjectileVisualPrefabDictionary : SerializableDictionary<ProjectileInstance.ProjectileTypeID, GameObject> { }

public class ProjectileManager : NetworkBehaviour
{
    #region fields
    [Header("Prefabs")]
    [SerializeField] private ProjectileInstancePrefabDictionary ProjectileInstancePrefabs;

    [SerializeField] private ProjectileVisualPrefabDictionary ProjectileVisualPrefabs;

    [Header("Pooling")]
    [SerializeField] private GameObject InstancePoolRoot;
    [SerializeField] private GameObject VisualPoolRoot;
    [SerializeField] private int InitialPoolSize = 10;

    [HideInInspector]
    public static ProjectileManager Singleton;

    #endregion

    #region internal Variables
    private Dictionary<ProjectileInstance.ProjectileTypeID, List<ProjectileInstance>> ProjectilePoolMap = 
                                new Dictionary<ProjectileInstance.ProjectileTypeID, List<ProjectileInstance>>();
    private Dictionary<ProjectileInstance.ProjectileTypeID, List<ProjectileVisual>> VisualPoolMap = 
                                new Dictionary<ProjectileInstance.ProjectileTypeID, List<ProjectileVisual>>();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;

        foreach (ProjectileInstance.ProjectileTypeID typeID in Enum.GetValues(typeof(ProjectileInstance.ProjectileTypeID)))
        {
            if (typeID == ProjectileInstance.ProjectileTypeID.INVALID)
            {
                continue;
            }
            IncreasePoolOfInstances(typeID, InitialPoolSize);
            IncreasePoolOfVisuals(typeID, InitialPoolSize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Public Interfaces

    public void IncreasePoolOfInstances(ProjectileInstance.ProjectileTypeID typeID, int amount)
    {
        if (!ProjectileInstancePrefabs.ContainsKey(typeID) || ProjectileInstancePrefabs[typeID] == null)
        {
            Debug.LogError("Projectile Instance " + typeID.ToString() + " tried to increase projectile pool size without valid prefab assigned");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            ProjectileInstance projectileInstance = GenerateProjectileInstance(typeID);
            if (!ProjectilePoolMap.ContainsKey(typeID))
            {
                ProjectilePoolMap.Add(typeID, new List<ProjectileInstance>());
            }
            ProjectilePoolMap[typeID].Add(projectileInstance);
            projectileInstance.transform.SetParent(InstancePoolRoot.transform);
        }
    }

    public void IncreasePoolOfVisuals(ProjectileInstance.ProjectileTypeID typeID, int amount)
    {
        if (!ProjectileVisualPrefabs.ContainsKey(typeID) || ProjectileVisualPrefabs[typeID] == null)
        {
            Debug.LogError("Projectile Visual " + typeID.ToString() + " tried to increase projectile visual pool size without valid prefab assigned");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            ProjectileVisual projectileVisual = GenerateProjectileVisual(typeID);
            if (!VisualPoolMap.ContainsKey(typeID))
            {
                VisualPoolMap.Add(typeID, new List<ProjectileVisual>());
            }
            VisualPoolMap[typeID].Add(projectileVisual);
            projectileVisual.transform.SetParent(VisualPoolRoot.transform);
        }
    }

    public ProjectileInstance ClaimProjectileInstance(ProjectileInstance.ProjectileTypeID typeID)
    {
        if (!ProjectilePoolMap.ContainsKey(typeID) || ProjectilePoolMap[typeID].Count == 0)
        {
            IncreasePoolOfInstances(typeID, 1);
        }

        // Check MAY seem redundant but the increase Pool instances function may have failed
        if (ProjectilePoolMap.ContainsKey(typeID) && ProjectilePoolMap[typeID].Count > 0)
        {
            ProjectileInstance projectileInstance = ProjectilePoolMap[typeID][0];
            ProjectilePoolMap[typeID].RemoveAt(0);
            return projectileInstance;
        }
        else
        {
            Debug.LogError("Failed to claim projectile instance of type " + typeID.ToString() + " even after increasing pool size");
        }

        return null;
    }

    public void FreeProjectileInstance(ProjectileInstance projectileInstance) 
    {
        // For this to be true, the projectile had to be created in a non-traditional way
        if (!ProjectilePoolMap.ContainsKey(projectileInstance.TypeID))
        {
            ProjectilePoolMap[projectileInstance.TypeID] = new List<ProjectileInstance>();
            Debug.LogWarning("Projectile Pool Map lacked entry for " + projectileInstance.TypeID.ToString() + " when freeing projectile instance");
        }

        ProjectilePoolMap[projectileInstance.TypeID].Add(projectileInstance);
        projectileInstance.transform.parent = InstancePoolRoot.transform;
        projectileInstance.gameObject.SetActive(false);
        projectileInstance.Cleanup();
    }

    public ProjectileVisual ClaimProjectileVisual(ProjectileInstance.ProjectileTypeID typeID)
    {
        if (!VisualPoolMap.ContainsKey(typeID) || VisualPoolMap[typeID].Count == 0)
        {
            IncreasePoolOfVisuals(typeID, 1);
        }

        if (VisualPoolMap.ContainsKey(typeID) && VisualPoolMap[typeID].Count > 0)
        {
            ProjectileVisual projectileVisual = VisualPoolMap[typeID][0];
            VisualPoolMap[typeID].RemoveAt(0);
            return projectileVisual;
        }
        else
        {
            Debug.LogError("Failed to claim projectile visual of type " + typeID.ToString() + " even after increasing pool size");
        }

        return null;
    }

    public void FreeProjectileVisual(ProjectileVisual projectileVisual)
    {
        // For this to be true, the projectile had to be created in a non-traditional way
        if (!VisualPoolMap.ContainsKey(projectileVisual.TypeID))
        {
            VisualPoolMap[projectileVisual.TypeID] = new List<ProjectileVisual>();
            Debug.LogWarning("Projectile Visual Pool Map lacked entry for " + projectileVisual.TypeID.ToString() + " when freeing projectile visual");
        }

        VisualPoolMap[projectileVisual.TypeID].Add(projectileVisual);
        projectileVisual.transform.parent = VisualPoolRoot.transform;
        projectileVisual.gameObject.SetActive(false);
        projectileVisual.Cleanup();
    }
    #endregion

    #region Internal Functions

    private ProjectileInstance GenerateProjectileInstance(ProjectileInstance.ProjectileTypeID typeID)
    {
        if (!ProjectileInstancePrefabs.ContainsKey(typeID) || ProjectileInstancePrefabs[typeID] == null)
        {
            Debug.LogError("Projectile Instance " + typeID.ToString() + " tried to generate projectile without valid prefab assigned");
            return null;
        }

        GameObject newProjectileObj = Instantiate(ProjectileInstancePrefabs[typeID]);
        ProjectileInstance projectileInstance = newProjectileObj.GetComponent<ProjectileInstance>();

        if (projectileInstance == null)
        {
            Debug.LogError("Projectile Instance " + typeID.ToString() + " Prefab lacks Projectile Instance script!");
            return null;
        }

        if (projectileInstance.TypeID != typeID)
        {
            Debug.LogWarning("Projectile Instance " + typeID.ToString() + " Prefab has mismatched type ID: " + projectileInstance.TypeID.ToString() + "! Force Fixed it!");
            projectileInstance.TypeID = typeID;
        }

        projectileInstance.ProjectileManager = this;
        projectileInstance.gameObject.SetActive(false);

        return projectileInstance;
    }

    private ProjectileVisual GenerateProjectileVisual(ProjectileInstance.ProjectileTypeID typeID)
    {
        if (!ProjectileVisualPrefabs.ContainsKey(typeID) || ProjectileVisualPrefabs[typeID] == null)
        {
            Debug.LogError("Projectile Visual " + typeID.ToString() + " tried to generate projectile visual without valid prefab assigned");
            return null;
        }

        GameObject newProjectileObj = Instantiate(ProjectileVisualPrefabs[typeID]);
        ProjectileVisual projectileVisual = newProjectileObj.GetComponent<ProjectileVisual>();
        if (projectileVisual == null)
        {
            Debug.LogError("Projectile Visual " + typeID.ToString() + " Prefab lacks Projectile Visual script!");
            return null;
        }

        if (projectileVisual.TypeID != typeID)
        {
            Debug.LogWarning("Projectile Visual " + typeID.ToString() + " Prefab has mismatched type ID! Force Fixed it!");
            projectileVisual.TypeID = typeID;
        }

        projectileVisual.ProjectileManager = this;

        projectileVisual.gameObject.SetActive(false);

        return projectileVisual;
    }
    #endregion
}
