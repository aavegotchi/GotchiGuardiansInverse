using UnityEngine;
using Gotchi.Events;

public class AreaOfEffectTower : BaseTower
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject areaOfEffectFieldPrefab = null;
    #endregion

    #region Private Variables
    private int numEnemyColliders = 0;
    private GameObject activatedAreaOfEffectField;
    private AreaOfEffectTowerObjectSO aoeTowerObjectSO;
    #endregion

    #region Unity Functions
    protected override void Start()
    {
        base.Start();

        aoeTowerObjectSO = towerObjectSO as AreaOfEffectTowerObjectSO;
        if (aoeTowerObjectSO != null)
        {
            GetComponent<SphereCollider>().radius = aoeTowerObjectSO.AreaOfEffectRange;
        }
    }

    void OnDisable()
    {
        if (activatedAreaOfEffectField == null) return;
        
        activatedAreaOfEffectField.SetActive(false);
    }
    #endregion

    #region Public Functions

    public override void OnEnemyEnter(Collider collider)
    {
        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Survival) return;

        var obj = collider.transform.gameObject;
        if (obj.tag != enemyTag) return;

        Enemy enemy = EnemyManager.Instance.GetEnemyByObject(obj);

        if (aoeTowerObjectSO != null)
        {
            enemy.AdjustEnemySpeed(enemy.MovementSpeed / aoeTowerObjectSO.SlowStrength);
        }

        numEnemyColliders++;

        bool isFirstEnemy = numEnemyColliders == 1;
        if (isFirstEnemy)
        {
            if (activatedAreaOfEffectField == null)
            {
                activatedAreaOfEffectField = Instantiate(areaOfEffectFieldPrefab, transform.position, transform.rotation);
            } 
            else if (activatedAreaOfEffectField.activeSelf)
            {
                return;
            }

            EventBus.TowerEvents.TowerAttacked(TowerManager.TowerType.SlowTower);
        }
    }

    #endregion

    #region Private Functions
    private void OnTriggerExit(Collider collider) // TODO: this doesn't get triggered when enemies die within the shield
    {
        var obj = collider.transform.gameObject;
        if (obj.tag != enemyTag) return;

        Enemy enemy = EnemyManager.Instance.GetEnemyByObject(obj);

        if (aoeTowerObjectSO != null)
        {
            enemy.AdjustEnemySpeed(enemy.MovementSpeed * aoeTowerObjectSO.SlowStrength);
        }

        numEnemyColliders--;

        if (numEnemyColliders == 0)
        {
            activatedAreaOfEffectField.SetActive(false);
        }
    }
    #endregion
}
