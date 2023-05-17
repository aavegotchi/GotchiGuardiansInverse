using System.Collections;
using UnityEngine;
using Gotchi.Events;
using PhaseManager;
using PhaseManager.Presenter;

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
         
        Enemy enemy = EnemyPool.Instance.GetEnemyByObject(obj);
        enemy.AdjustEnemySpeed(enemy.ObjectSO.MovementSpeed / slowStrength);
        numEnemyColliders++;

        bool isFirstEnemy = numEnemyColliders == 1;
        if (isFirstEnemy)
        {
            activatedSlowField = Instantiate(slowField, transform.position, transform.rotation);
            
            EventBus.TowerEvents.TowerAttacked(TowerPool.TowerType.SlowTower);
        }
    }
    #endregion
}
