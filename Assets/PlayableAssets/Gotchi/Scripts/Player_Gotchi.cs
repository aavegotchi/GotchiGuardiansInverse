using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gotchi.Events;
using Fusion;
using PhaseManager;
using PhaseManager.Presenter;

public class Player_Gotchi : NetworkBehaviour, IDamageable
{
    #region Public Variables
    public bool IsDead
    {
        get { return isDead; }
    }
    #endregion

    #region Fields
    [Header("Settings")]
    [SerializeField] private GotchiObjectSO gotchiObjectSO = null;

    [Header("Required Refs")]
    [SerializeField] private Animator swordAnim = null;
    [SerializeField] private Animator spinAnim = null;
    [SerializeField] private GameObject swordEffect = null;
    [SerializeField] private GameObject spinEffect = null;
    [SerializeField] private HealthBar_UI healthbar = null;
    [SerializeField] private Transform HealthbarOffset = null;
    [SerializeField] private RangeCircle rangeCircle = null;

    [Header("Attributes")]
    [SerializeField] private string swordTrigger = "Swing";
    [SerializeField] private string spinTrigger = "Spin";
    [SerializeField] private string enemyTag = "Enemy";
    #endregion

    #region Private Variables
    private float attackCountdownTracker = 1f;
    private Transform target = null;
    private Enemy targetEnemy = null;
    private bool isDead = false;
    #endregion

    #region Unity Functions
    void Start()
    {
        InvokeRepeating("updateTarget", 1f, 1f);

        Invoke("assignHealthBar", 2f);

        rangeCircle.SetScale(gotchiObjectSO.SpinAbilityRange);
    }

    void Update()
    {
        if (target == null) return;
        if (PhasePresenter.Instance.GetCurrentPhase() != PhaseManager.Phase.Survival) return;
        
        LockOntoTargetPos(target.position);
        attack();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gotchiObjectSO.AttackRange);
    }
    #endregion

    #region RPC Functions
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void rpc_animateSpinAttack()
    {
        spinAnim.SetTrigger(spinTrigger);
        spinEffect.SetActive(true);
        EventBus.GotchiEvents.GotchiAttacked(GotchiManager.AttackType.Spin);
    } 
    #endregion

    #region Public Functions
    public void SpinAttack()
    {
        PlayerStoppedHoveringMouseOverAbility();
        rpc_animateSpinAttack();

        if (PhasePresenter.Instance.GetCurrentPhase() == Phase.Prep) return;

        List<GameObject> enemyObjects = EnemyPool.Instance.ActiveEnemies;
        Enemy[] enemies = EnemyPool.Instance.GetEnemiesByObjects(enemyObjects.ToArray());

        foreach (Enemy enemy in enemies)
        {
            float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            bool isInRange = distanceToTarget < (gotchiObjectSO.SpinAbilityRange * 2);
            if (isInRange)
            {
                enemy.Damage(gotchiObjectSO.SpinAbilityDamage);

                Vector3 direction = (enemy.transform.position - transform.position).normalized;
                enemy.Knockback(direction * gotchiObjectSO.SpinAbilityKnockbackForce);
            }
        }
    }

    public void Damage(float damage)
    {
        if (isDead) return;

        healthbar.CurrentHealth -= damage;
        healthbar.ShowDamagePopUpAndColorDifferentlyIfEnemy(damage, false);

        if (healthbar.CurrentHealth <= 0) playDead();
    }

    public void LockOntoTargetPos(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * gotchiObjectSO.AttackRotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void PlayerHoveredMouseOverAbility() {
        rangeCircle.ToggleActive(true);
    }

    public void PlayerStoppedHoveringMouseOverAbility()
    {
        rangeCircle.ToggleActive(false);
    }
    #endregion

    #region Private Functions
    private void assignHealthBar()
    {
        healthbar = HealthBarPool_UI.Instance.GetHealthbar(HealthbarOffset);
        healthbar.SetHealthbarMaxHealth(gotchiObjectSO.Health);
    }
    
    private void attack()
    {
        bool isAttacking = attackCountdownTracker > 0f;
        if (isAttacking)
        {
            attackCountdownTracker -= Time.deltaTime;
            return;
        }
        
        swordAnim.SetTrigger(swordTrigger);
        swordEffect.SetActive(true);

        EventBus.GotchiEvents.GotchiAttacked(GotchiManager.AttackType.Basic);

        targetEnemy.Damage(gotchiObjectSO.AttackDamage);

        attackCountdownTracker = gotchiObjectSO.AttackCountdown;
    }

    private void updateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag).Where(enemy => enemy.activeSelf).ToArray();
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            bool isCloserTarget = distanceToTarget < shortestDistance;
            if (isCloserTarget)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = enemy;
            }
        }

        bool isClosestTarget = nearestTarget != null && shortestDistance <= gotchiObjectSO.AttackRange;
        if (isClosestTarget)
        {
            target = nearestTarget.transform;
            targetEnemy = target.GetComponent<Enemy>();
            return;
        }

        target = null;
    }

    private void playDead()
    {
        EventBus.GotchiEvents.GotchiDied();
        
        isDead = true;
        gameObject.SetActive(false);
        healthbar.Reset();
        healthbar = null;

        GameObject[] gotchis = GameObject.FindGameObjectsWithTag("Tower")
            .Where(gotchi => gotchi.activeSelf && gotchi.GetComponent<Player_Gotchi>() != null && !gotchi.GetComponent<Player_Gotchi>().IsDead).ToArray();
        
        if (gotchis.Length <= 0)
        {
            EventBus.GotchiEvents.GotchisAllDead();
        }
    }
    #endregion
}
