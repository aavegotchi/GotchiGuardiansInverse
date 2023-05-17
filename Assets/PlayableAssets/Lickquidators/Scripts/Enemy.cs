using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gotchi.Events;
using Gotchi.Network;
using PhaseManager;
using PhaseManager.Presenter;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Public Variables
    public Transform HealthbarOffset
    {
        get { return healthbarOffset; }
    }

    public EnemyBlueprint EnemyBlueprint
    {
        get { return enemyBlueprint; }
        set { enemyBlueprint = value; }
    }

    public LickquidatorObjectSO ObjectSO
    {
        get { return lickquidatorObjectSO; }
        set { lickquidatorObjectSO = value; }
    }
    #endregion

    #region Fields
    [Header("Settings")]
    [SerializeField] private LickquidatorObjectSO lickquidatorObjectSOOriginal = null;
    [SerializeField] private GeneralSO generalSO = null;

    [Header("Required Refs")]
    [SerializeField] private HealthBar_UI healthbar = null;
    [SerializeField] private Transform healthbarOffset = null;

    [Header("Attributes")]
    [SerializeField] private ImpactPool_FX.ImpactType deathEffect = ImpactPool_FX.ImpactType.BasicTower;
    #endregion

    #region Private Variables
    private NavMeshAgent agent = null;
    private EnemyBlueprint enemyBlueprint = null;
    private BoxCollider boxCollider = null;
    private Rigidbody body = null;
    private LickquidatorObjectSO lickquidatorObjectSO = null;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();

        lickquidatorObjectSO = ScriptableObject.CreateInstance<LickquidatorObjectSO>();
        resetScriptableObject();
    }

    void Update()
    {
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Survival) return;

        goAfterGotchi();
    }

    // void FixedUpdate()
    // {
    //     listenForRigidbodyClicks();
    // }

    void Start()
    {
        setNavMeshAgentFields();
    }

    void OnMouseDown()
    {
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Prep) return;

        NodeManager.Instance.NodeUI.OpenNodeUpgradeUI(transform, this);
    }
    #endregion

    #region Public Functions
    public void AdjustEnemySpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetHealthbarAndResetHealth(HealthBar_UI _healthbar)
    {
        healthbar = _healthbar;
        healthbar.SetHealthbarMaxHealth(lickquidatorObjectSO.Health);
    }

    public void Damage(float damage)
    {
        if (healthbar == null) return;

        healthbar.CurrentHealth -= damage;
        healthbar.ShowDamagePopUpAndColorDifferentlyIfEnemy(damage, true);

        if (healthbar.CurrentHealth <= 0)
        {
            PlayDead(true);
            StatsManager.Instance.TrackKillEnemy(enemyBlueprint);
        }
    }

    public void Knockback(Vector3 force)
    {
        body.AddForce(force, ForceMode.Impulse);
    }

    public void PlayDead(bool keepUpgrades = false)
    {
        EventBus.EnemyEvents.EnemyDied(enemyBlueprint.type);
        ImpactPool_FX.Instance.SpawnImpact(deathEffect, transform.position, transform.rotation);

        gameObject.SetActive(false);
        if (healthbar != null)
        {
            healthbar.Reset();
            healthbar = null;
        }

        if (!keepUpgrades)
        {
            resetScriptableObject();
        }

        float value = lickquidatorObjectSO.Cost / generalSO.EnemyKillRewardMultipleByCost;
        int roundedValue = Mathf.RoundToInt(value / 5.0f) * 5;
        StatsManager.Instance.Money += roundedValue;
    }

    public void Freeze()
    {
        agent.enabled = false;
        // boxCollider.enabled = false;
    }

    public void Unfreeze()
    {
        agent.enabled = true;
        // boxCollider.enabled = true;
    }
    #endregion

    #region Private Functions
    // private void listenForRigidbodyClicks()
    // {
    //     int layerToHit = 1 << 6; // only register clicks for "Lickquidator" layer (#6)
    //     // NOTE: to register clicks for everything other than a specific layer, invert using "layerToHit = ~layerToHit;"

    //     RaycastHit hit;

    //     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerToHit))
    //     {
    //         if (hit.rigidbody != null)
    //         {
    //             hit.rigidbody.gameObject.SendMessage("OnMouseDown");
    //         }
    //         else
    //         {
    //             hit.collider.SendMessage("OnMouseDown");
    //         }
    //     }
    // }

    private void setNavMeshAgentFields()
    {
        agent.speed = lickquidatorObjectSO.MovementSpeed;
        agent.acceleration = lickquidatorObjectSO.MovementAcceleration;
        agent.angularSpeed = lickquidatorObjectSO.AngularSpeed;
        agent.radius = lickquidatorObjectSO.NavMeshAgentRadius;
        agent.height = lickquidatorObjectSO.NavMeshAgentHeight;
        agent.avoidancePriority = lickquidatorObjectSO.NavMeshAgentPriority;
    }

    private void goAfterGotchi()
    {
        if (PhasePresenter.Instance.GetCurrentPhase() == Phase.Survival)
        {
            if (!agent.enabled) agent.enabled = true;
        }

        // if (!NetworkManager.Instance.LocalPlayerGotchi.IsDead)
        // {
        //     agent.SetDestination(NetworkManager.Instance.LocalPlayerGotchi.transform.position);
        // }
        // else
        // {
        //     healthbar.Reset();
        //     healthbar = null;
        //     gameObject.SetActive(false);
        // }
    }

    private void resetScriptableObject()
    {
        lickquidatorObjectSO.Name = lickquidatorObjectSOOriginal.Name;
        lickquidatorObjectSO.Type = lickquidatorObjectSOOriginal.Type;
        lickquidatorObjectSO.Level = lickquidatorObjectSOOriginal.Level;
        lickquidatorObjectSO.AttackDamage = lickquidatorObjectSOOriginal.AttackDamage;
        lickquidatorObjectSO.AttackRange = lickquidatorObjectSOOriginal.AttackRange;
        lickquidatorObjectSO.AttackCountdown = lickquidatorObjectSOOriginal.AttackCountdown;
        lickquidatorObjectSO.Cost = lickquidatorObjectSOOriginal.Cost;
        lickquidatorObjectSO.buildTime = lickquidatorObjectSOOriginal.buildTime;
        lickquidatorObjectSO.Health = lickquidatorObjectSOOriginal.Health;
        lickquidatorObjectSO.OffsetDistance = lickquidatorObjectSOOriginal.OffsetDistance;
        lickquidatorObjectSO.MovementSpeed = lickquidatorObjectSOOriginal.MovementSpeed;
        lickquidatorObjectSO.MovementAcceleration = lickquidatorObjectSOOriginal.MovementAcceleration;
        lickquidatorObjectSO.AngularSpeed = lickquidatorObjectSOOriginal.AngularSpeed;
        lickquidatorObjectSO.AttackRotationSpeed = lickquidatorObjectSOOriginal.AttackRotationSpeed;
        lickquidatorObjectSO.NavMeshAgentHeight = lickquidatorObjectSOOriginal.NavMeshAgentHeight;
        lickquidatorObjectSO.NavMeshAgentPriority = lickquidatorObjectSOOriginal.NavMeshAgentPriority;
        lickquidatorObjectSO.NavMeshAgentRadius = lickquidatorObjectSOOriginal.NavMeshAgentRadius;
        lickquidatorObjectSO.NavMeshAgentStoppingDistance = lickquidatorObjectSOOriginal.NavMeshAgentStoppingDistance;
    }
    #endregion
}