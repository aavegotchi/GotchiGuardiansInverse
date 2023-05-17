using UnityEngine;
using UnityEngine.AI;
using Gotchi.Audio;
using System.Collections;
using System.Linq;
using PhaseManager;
using PhaseManager.Presenter;

public abstract class BaseEnemy : MonoBehaviour
{
    #region Fields
    [Header("Required References")]
    [SerializeField] protected Animator anim = null;
    protected NavMeshAgent agent = null;
    [SerializeField] protected GameObject attackParticleEffectGO = null;

    [Header("Attributes")]
    [SerializeField] protected string attackTrigger = "Attack";
    [SerializeField] protected string towerTag = "Tower";
    #endregion

    #region Private Variables
    protected float attackCountdownTracker = 1f;
    protected Transform target = null;
    protected IDamageable currentTarget = null;
    protected Enemy enemy = null;
    #endregion

    #region Unity Functions
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    protected virtual void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 1f);
        InvokeRepeating("UpdateGotchiTarget", 0f, 1f);
    }

    protected virtual void Update()
    {
        if (target == null) return;
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Survival) return;

        LockOntoTarget();
        StartCoroutine(Attack());
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy.ObjectSO.AttackRange);
    }
    #endregion

    #region Protected Functions
    protected virtual IEnumerator Attack()
    {
        bool isAttacking = attackCountdownTracker > 0f;
        if (isAttacking)
        {
            attackCountdownTracker -= Time.deltaTime;
            yield break;
        }

        attackCountdownTracker = enemy.ObjectSO.AttackCountdown;

        if(anim != null) anim.SetTrigger(attackTrigger);
        if(attackParticleEffectGO != null) attackParticleEffectGO.SetActive(true);
        OnAttackSound();
        currentTarget.Damage(enemy.ObjectSO.AttackDamage);
    }

    protected virtual void LockOntoTarget()
    {
        agent.isStopped = true;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * enemy.ObjectSO.AttackRotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    protected virtual void UpdateTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag)
            .Where(tower => tower.activeSelf).ToArray();
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject tower in towers)
        {
            float distanceToTarget = Vector3.Distance(transform.position, tower.transform.position);
            bool isCloserTarget = distanceToTarget < shortestDistance;
            if (isCloserTarget)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = tower;
            }
        }

        bool isClosestTarget = nearestTarget != null && shortestDistance <= enemy.ObjectSO.AttackRange;
        if (isClosestTarget)
        {
            target = nearestTarget.transform;
            if(currentTarget == null)
                currentTarget = target.GetComponent<IDamageable>();
            return;
        }

        target = null;
        currentTarget = null;

        if (agent.enabled && agent.isOnNavMesh) 
        {
            agent.isStopped = false;
        }
    }

    protected virtual void UpdateGotchiTarget()
    {
        GameObject[] gotchis = GameObject.FindGameObjectsWithTag(towerTag)
            .Where(gotchi => gotchi.activeSelf && gotchi.GetComponent<Player_Gotchi>() != null && !gotchi.GetComponent<Player_Gotchi>().IsDead).ToArray();
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject gotchi in gotchis)
        {
            float distanceToTarget = Vector3.Distance(transform.position, gotchi.transform.position);
            bool isCloserTarget = distanceToTarget < shortestDistance;
            if (isCloserTarget)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = gotchi;
            }
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(nearestTarget.transform.position);
        }
    }

    protected virtual void OnAttackSound() { }
    #endregion
}
