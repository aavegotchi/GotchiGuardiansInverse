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
    private bool isDead = false;
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
        
        openNodeUI();
    }
    #endregion

    #region Public Functions
    public void Damage(float damage)
    {
        if (isDead) return;

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
        isDead = true;
        gameObject.SetActive(false);
        if (healthbar != null) 
        {
            healthbar.Reset();
            healthbar = null;
        }
    }
    #endregion

    #region Private Functions
    private void openNodeUI()
    {
        Vector3 nodeUIPosition = transform.position;
        nodeUIPosition.y = 25f;
        nodeUIPosition.z -= 8f;

        NodeUI nodeUI = NodeManager.Instance.NodeUI;
        nodeUI.transform.position = nodeUIPosition;

        nodeUI.Close();
        nodeUI.UpgradeInventory.Open(towerObjectSO, transform);
        nodeUI.Open();
    }  

    private void SetupHealthBar()
    {
        healthbar = HealthBarManager.Instance.GetHealthbar(healthbarOffset);
        healthbar.CurrentHealth = towerObjectSO.Health;
        healthbar.MaxHealth = towerObjectSO.Health;
    }
    #endregion
}
