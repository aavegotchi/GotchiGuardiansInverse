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
    [SerializeField] protected BaseTowerObjectSO towerObjectSO = null;

    [Header("Required Refs")]
    [SerializeField] private Transform healthbarOffset = null;

    [Header("Attributes")]
    [SerializeField] protected string enemyTag = "Enemy";
    [SerializeField] protected ImpactManager.ImpactType impactType = ImpactManager.ImpactType.BasicTower;
    #endregion

    #region Private Variables
    private HealthBar_UI healthbar = null;
    protected TowerBlueprint towerBlueprint = null;
    #endregion

    #region Unity Functions
    protected virtual void Start()
    {
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

    public void PlayDead()
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
    }
    #endregion

    #region Private Functions
    private void SetupHealthBar()
    {
        healthbar = HealthBarManager.Instance.GetHealthbar(healthbarOffset);
        healthbar.CurrentHealth = towerObjectSO.Health;
        healthbar.MaxHealth = towerObjectSO.Health;
    }
    #endregion
}
