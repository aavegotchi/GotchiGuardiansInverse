using UnityEngine;
using UnityEngine.AI;
using Gotchi.Audio;
using System.Collections;
using System.Linq;

public abstract class BaseEnemy : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private LickquidatorObjectSO lickquidatorObjectSO = null;

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
    #endregion

    #region Unity Functions
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    protected virtual void Update()
    {
        if (target == null) return;
        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Survival) return;

        LockOntoTarget();
        StartCoroutine(Attack());
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lickquidatorObjectSO.AttackRange);
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

        attackCountdownTracker = lickquidatorObjectSO.AttackCountdown;

        if(anim != null) anim.SetTrigger(attackTrigger);
        if(attackParticleEffectGO != null) attackParticleEffectGO.SetActive(true);
        OnAttackSound();
        currentTarget.Damage(lickquidatorObjectSO.AttackDamage);
    }

    protected virtual void LockOntoTarget()
    {
        agent.isStopped = true;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lickquidatorObjectSO.AttackRotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    protected virtual void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(towerTag)
            .Where(enemy => enemy.activeSelf).ToArray();
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

        bool isClosestTarget = nearestTarget != null && shortestDistance <= lickquidatorObjectSO.AttackRange;
        if (isClosestTarget)
        {
            target = nearestTarget.transform;
            if(currentTarget == null)
                currentTarget = target.GetComponent<IDamageable>();
            return;
        }

        target = null;
        currentTarget = null;

        if(agent.enabled) agent.isStopped = false;
    }

    protected virtual void OnAttackSound() { }
    #endregion
}
