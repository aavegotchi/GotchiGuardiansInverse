using UnityEngine;
using Gotchi.Events;

public abstract class BaseTower : MonoBehaviour, IDamageable
{
    #region Public Variables
    public Transform HealthbarOffset
    {
        get { return healthbarOffset; }
    }

    public TowerBlueprint TowerBlueprint
    {
        get { return towerBlueprint; }
        set { towerBlueprint = value; }
    }

    public BaseTowerObjectSO ObjectSO
    {
        get { return towerObjectSO; }
        set { towerObjectSO = value; }
    }
    #endregion

    #region Fields
    [Header("Settings")]
    [SerializeField] protected BaseTowerObjectSO towerObjectSOOriginal = null;

    [Header("Required Refs")]
    [SerializeField] private Transform healthbarOffset = null;

    [Header("Attributes")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected ImpactManager.ImpactType impactType = ImpactManager.ImpactType.BasicTower;
    #endregion

    #region Private Variables
    private HealthBar_UI healthbar = null;
    protected TowerBlueprint towerBlueprint = null;
    protected BaseTowerObjectSO towerObjectSO = null;
    #endregion

    #region Unity Functions
    protected virtual void Awake()
    {
        if (towerObjectSOOriginal is TurretTowerObjectSO)
        {
            towerObjectSO = ScriptableObject.CreateInstance<TurretTowerObjectSO>();
            resetScriptableObjectTurret();
        }
        else if (towerObjectSOOriginal is AreaOfEffectTowerObjectSO)
        {
            towerObjectSO = ScriptableObject.CreateInstance<AreaOfEffectTowerObjectSO>();
            resetScriptableObjectAoe();
        }
    }

    protected virtual void Start()
    {
        if (enemyTag == "Tower") return; // TODO: messy atm as this is prevent tower's upgrade ui for the aerial lickquidator

        SetupHealthBar();
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        OnEnemyEnter(collider);
    }

    void OnMouseDown()
    {
        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Prep) return;
        if (enemyTag == "Tower") return; // TODO: messy atm as this is prevent tower's upgrade ui for the aerial lickquidator

        NodeManager.Instance.NodeUI.OpenNodeUpgradeUI(transform, this);
    }
    #endregion

    #region Public Functions
    public void Damage(float damage)
    {
        if (healthbar == null) return;

        healthbar.CurrentHealth -= damage;
        healthbar.ShowDamagePopUpAndColorDifferentlyIfEnemy(damage, false);

        if (healthbar.CurrentHealth <= 0) 
        {
            PlayDead();
            StatsManager.Instance.TrackKillTower(towerBlueprint);
        }
    }

    public abstract void OnEnemyEnter(Collider collider);

    public void PlayDead(bool keepUpgrades = false)
    {
        EventBus.TowerEvents.TowerDied(towerBlueprint.type);
        towerBlueprint.node.SetOccupiedStatusToFalse();
        ImpactManager.Instance.SpawnImpact(impactType, transform.position, transform.rotation);
        
        gameObject.SetActive(false);
        if (healthbar != null) 
        {
            healthbar.Reset();
            healthbar = null;
        }

        if (!keepUpgrades)
        {
            if (towerObjectSOOriginal is TurretTowerObjectSO)
            {
                resetScriptableObjectTurret();
            }
            else if (towerObjectSOOriginal is AreaOfEffectTowerObjectSO)
            {
                resetScriptableObjectAoe();
            }
        }
    }
    #endregion

    #region Private Functions
    private void SetupHealthBar()
    {
        healthbar = HealthBarManager.Instance.GetHealthbar(healthbarOffset);
        healthbar.CurrentHealth = towerObjectSO.Health;
        healthbar.MaxHealth = towerObjectSO.Health;
    }

    private void resetScriptableObjectTurret()
    {
        towerObjectSO.Name = towerObjectSOOriginal.Name;
        towerObjectSO.Type = towerObjectSOOriginal.Type;
        towerObjectSO.Cost = towerObjectSOOriginal.Cost;
        towerObjectSO.buildTime = towerObjectSOOriginal.buildTime;
        towerObjectSO.Health = towerObjectSOOriginal.Health;

        ((TurretTowerObjectSO)towerObjectSO).AttackRange = ((TurretTowerObjectSO)towerObjectSOOriginal).AttackRange;
        ((TurretTowerObjectSO)towerObjectSO).AttackCountdown = ((TurretTowerObjectSO)towerObjectSOOriginal).AttackCountdown;
        ((TurretTowerObjectSO)towerObjectSO).AttackDamageMultiplier = ((TurretTowerObjectSO)towerObjectSOOriginal).AttackDamageMultiplier;
        ((TurretTowerObjectSO)towerObjectSO).AttackRotationSpeed = ((TurretTowerObjectSO)towerObjectSOOriginal).AttackRotationSpeed;
        ((TurretTowerObjectSO)towerObjectSO).deathEffectType = ((TurretTowerObjectSO)towerObjectSOOriginal).deathEffectType;
        ((TurretTowerObjectSO)towerObjectSO).projectile = ((TurretTowerObjectSO)towerObjectSOOriginal).projectile;
    }

    private void resetScriptableObjectAoe()
    {
        towerObjectSO.Name = towerObjectSOOriginal.Name;
        towerObjectSO.Type = towerObjectSOOriginal.Type;
        towerObjectSO.Cost = towerObjectSOOriginal.Cost;
        towerObjectSO.buildTime = towerObjectSOOriginal.buildTime;
        towerObjectSO.Health = towerObjectSOOriginal.Health;

        ((AreaOfEffectTowerObjectSO)towerObjectSO).AreaOfEffectRange = ((AreaOfEffectTowerObjectSO)towerObjectSOOriginal).AreaOfEffectRange;
        ((AreaOfEffectTowerObjectSO)towerObjectSO).SlowStrength = ((AreaOfEffectTowerObjectSO)towerObjectSOOriginal).SlowStrength;
        ((AreaOfEffectTowerObjectSO)towerObjectSO).deathEffectType = ((AreaOfEffectTowerObjectSO)towerObjectSOOriginal).deathEffectType;
    }
    #endregion
}
