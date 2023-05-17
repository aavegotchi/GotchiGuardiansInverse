using UnityEngine;
using Gotchi.Events;
using PhaseManager;
using PhaseManager.Presenter;

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
    [SerializeField] protected ImpactPool_FX.ImpactType impactType = ImpactPool_FX.ImpactType.BasicTower;

    [SerializeField] private GameObject rangeCirclePrefab;
    #endregion

    #region Private Variables
    private HealthBar_UI healthbar = null;
    protected TowerBlueprint towerBlueprint = null;
    protected BaseTowerObjectSO towerObjectSO = null;
    protected RangeCircle rangeCircle = null;
    protected bool isNodeUIOpenOnThis = false;
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

        setupHealthBar();
        setupRangeCircle();
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
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Prep) return;
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

    public void OverrideRangeCircle(bool nextStatus)
    {
        if(nextStatus)
        {
            if (!isNodeUIOpenOnThis)
            {
                isNodeUIOpenOnThis = true;

                if (rangeCircle != null)
                {
                    rangeCircle.ToggleActive(true);
                }
            }
        }

        else
        {
            isNodeUIOpenOnThis = false;
        }
    }

    public abstract void OnEnemyEnter(Collider collider);

    public void PlayDead(bool keepUpgrades = false)
    {
        EventBus.TowerEvents.TowerDied(towerBlueprint.type);
        towerBlueprint.node.Occupied = keepUpgrades;
        ImpactPool_FX.Instance.SpawnImpact(impactType, transform.position, transform.rotation);
        
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
    private void setupHealthBar()
    {
        healthbar = HealthBarPool_UI.Instance.GetHealthbar(healthbarOffset);
        healthbar.CurrentHealth = towerObjectSO.Health;
        healthbar.MaxHealth = towerObjectSO.Health;
    }

    private void setupRangeCircle()
    {
        GameObject rangeCircleInstance = Instantiate(rangeCirclePrefab, transform);
        rangeCircleInstance.transform.localPosition = new Vector3(0f, .2f, 0f);

        rangeCircle = rangeCircleInstance.GetComponent<RangeCircle>();

        if (towerObjectSO is TurretTowerObjectSO)
        {
            rangeCircle.SetScale(((TurretTowerObjectSO)towerObjectSO).AttackRange);
        }
        else if (towerObjectSO is AreaOfEffectTowerObjectSO)
        {
            rangeCircle.SetScale(((AreaOfEffectTowerObjectSO)towerObjectSO).AreaOfEffectRange);
        }
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
