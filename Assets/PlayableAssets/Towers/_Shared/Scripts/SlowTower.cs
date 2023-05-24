using UnityEngine;
using GameMaster;
using PhaseManager;
using PhaseManager.Presenter;
using Gotchi.Lickquidator.Manager;
using Gotchi.Lickquidator.Presenter;

public class SlowTower : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject slowField = null;

    [Header("Attributes")]
    [SerializeField] private float slowStrength = 1.5f;
    #endregion

    #region Private Variables
    private float range = 40f;
    private int numEnemyColliders = 0;
    private GameObject activatedSlowField = null;
    #endregion

    #region Unity Functions
    void Start()
    {
        slowField.transform.localScale = new Vector3(range, range, range);
        GetComponent<SphereCollider>().radius = range;
    }

    void OnDisable()
    {
        Destroy(activatedSlowField); // TODO: change to pooling
    }

    void OnTriggerEnter(Collider collider)
    {
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Survival) return;
        
        var obj = collider.transform.gameObject;
        if (obj.tag != "Enemy") return;
         
        LickquidatorPresenter enemy = LickquidatorManager.Instance.GetByObject(obj);
        enemy.Model.UpdateMovementSpeed(enemy.Model.MovementSpeed / slowStrength);
        numEnemyColliders++;

        bool isFirstEnemy = numEnemyColliders == 1;
        if (isFirstEnemy)
        {
            activatedSlowField = Instantiate(slowField, transform.position, transform.rotation);
            
            GameMasterEvents.TowerEvents.TowerAttacked(TowerPool.TowerType.SlowTower);
        }
    }
    #endregion
}
