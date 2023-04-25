using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gotchi.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Public Variables
    public float MovementSpeed
    {
        get { return lickquidatorObjectSO.MovementSpeed; }
    }

    public Transform HealthbarOffset
    {
        get { return healthbarOffset; }
    }

    public EnemyBlueprint EnemyBlueprint
    {
        get { return enemyBlueprint; }
        set { enemyBlueprint = value; }
    }
    #endregion

    #region Fields
    [Header("Settings")]
    [SerializeField] private LickquidatorObjectSO lickquidatorObjectSO = null;
    [SerializeField] private GeneralSO generalSO = null;

    [Header("Required Refs")]
    [SerializeField] private HealthBar_UI healthbar = null;
    [SerializeField] private Transform healthbarOffset = null;
   
    [Header("Attributes")]
    [SerializeField] private ImpactManager.ImpactType deathEffect = ImpactManager.ImpactType.BasicTower;
    #endregion
    
    #region Private Variables
    private NavMeshAgent agent = null;
    private EnemyBlueprint enemyBlueprint = null;
    private BoxCollider boxCollider = null;
    private Rigidbody body = null;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Survival) return;

        goAfterGotchi();
    }

    void Start()
    {
        setNavMeshAgentFields();
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
            PlayDead();
            StatsManager.Instance.TrackKillEnemy(enemyBlueprint);
        }
    }

    public void Knockback(Vector3 force)
    {
        body.AddForce(force, ForceMode.Impulse);
    }

    public void PlayDead()
    {
        ImpactManager.Instance.SpawnImpact(deathEffect, transform.position, transform.rotation);
        
        EventBus.EnemyEvents.EnemyDied(enemyBlueprint.type);        
        
        gameObject.SetActive(false);
        healthbar.Reset();
        healthbar = null;

        float value = lickquidatorObjectSO.Cost / generalSO.EnemyKillRewardMultipleByCost;
        int roundedValue = Mathf.RoundToInt(value / 5.0f) * 5;
        StatsManager.Instance.Money += roundedValue;
    }
    #endregion

    #region Private Functions
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
        if (PhaseManager.Instance.CurrentPhase == PhaseManager.Phase.Survival)
        {
            if (!agent.enabled) agent.enabled = true;
        }

        if (GotchiManager.Instance != null && GotchiManager.Instance.Player != null)
        {
            agent.SetDestination(GotchiManager.Instance.Player.transform.position);
        } else
        {
            healthbar.Reset();
            healthbar = null;
            gameObject.SetActive(false);
        }
    }


    // Add the Freeze function
    public void Freeze()
    {
        agent.enabled = false;
        boxCollider.enabled = false;

    }

    // Add the Unfreeze function
    public void Unfreeze()
    {
        agent.enabled = true;
        boxCollider.enabled = true;
    }

    #endregion
}
