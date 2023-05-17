using UnityEngine;
using Gotchi.Events;
using PhaseManager;
using PhaseManager.Presenter;

public class AreaOfEffectTower : BaseTower
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject areaOfEffectFieldPrefab = null;
    #endregion

    #region Private Variables
    private int numEnemyColliders = 0;
    private GameObject activatedAreaOfEffectField = null;
    private AreaOfEffectTowerObjectSO aoeTowerObjectSO = null;
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
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Survival) return;

        var obj = collider.transform.gameObject;
        if (obj.tag != enemyTag) return;

        Enemy enemy = EnemyPool.Instance.GetEnemyByObject(obj);

        if (aoeTowerObjectSO != null)
        {
            enemy.AdjustEnemySpeed(enemy.ObjectSO.MovementSpeed / aoeTowerObjectSO.SlowStrength);
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

            EventBus.TowerEvents.TowerAttacked(TowerPool.TowerType.SlowTower);
        }
    }

    #endregion

    #region Private Functions
    private void OnTriggerExit(Collider collider) // TODO: this doesn't get triggered when enemies die within the shield
    {
        var obj = collider.transform.gameObject;
        if (obj.tag != enemyTag) return;

        Enemy enemy = EnemyPool.Instance.GetEnemyByObject(obj);

        if (aoeTowerObjectSO != null)
        {
            enemy.AdjustEnemySpeed(enemy.ObjectSO.MovementSpeed * aoeTowerObjectSO.SlowStrength);
        }

        numEnemyColliders--;

        if (numEnemyColliders == 0)
        {
            activatedAreaOfEffectField.SetActive(false);
        }
    }
    #endregion
}
