using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Gotchi.Events;

namespace Gotchi.New
{
    public class LickquidatorPresenter : MonoBehaviour
    {
        public LickquidatorModel Model { get { return model; } }

        [Header("Model")]
        [SerializeField] private LickquidatorModel model = null;

        [Header("View")]
        [SerializeField] private ImpactPool_FX.ImpactType deathEffect = ImpactPool_FX.ImpactType.BasicTower;
        [SerializeField] private Animator attackAnimation = null;
        [SerializeField] private GameObject attackParticleEffect = null;

        [Header("Gameplay")]
        [SerializeField] private int frameReadyInterval = 10;

        private HealthBar_UI healthBar = null;
        private NavMeshAgent agent = null;
        private Transform inRangeTargetTransform = null;
        private IDamageable inRangeTarget = null;
        private float attackCountdownTracker = 1f;
        private Rigidbody rigidBody = null;

        #region Unity Functions
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            configureAgent();
        }

        void OnEnable()
        {
            model.OnMovementSpeedUpdated += handleOnMovementSpeedUpdated;
            model.OnHealthUpdated += handleOnHealthUpdated;
            model.OnIsMovingUpdated += handleIsMovingUpdated;
        }

        void OnDisable()
        {
            model.OnMovementSpeedUpdated -= handleOnMovementSpeedUpdated;
            model.OnHealthUpdated -= handleOnHealthUpdated;
            model.OnIsMovingUpdated -= handleIsMovingUpdated;
        }

        void Update()
        {
            if (!isSurvivalPhase() || !isFrameReady()) return;

            updateInRangeTarget();
            updateUltimateTarget();

            if (model.IsPassive || !inRangeTargetTransform) return;

            rotateTowardInRangeTarget();
            attackInRangeTarget();
        }

        void OnMouseDown()
        {
            if (!isPrepPhase()) return;

            NodeManager.Instance.NodeUI.OpenNodeUpgradeUI(transform, model);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = model.RangeIndicatorColor;
            Gizmos.DrawWireSphere(transform.position, model.Config.AttackRange);
        }
        #endregion

        #region Public Functions
        public void Damage(int damage)
        {
            model.UpdateHealth(model.Health - damage);
        }

        public void AssignHealthBar(HealthBar_UI healthBar)
        {
            this.healthBar = healthBar;
            this.healthBar.SetHealthbarMaxHealth(model.Health);
        }

        public void PlayDead(bool keepUpgrades = false)
        {
            EventBus.EnemyEvents.EnemyDied(model.EnemyBlueprint.type);
            ImpactPool_FX.Instance.SpawnImpact(deathEffect, transform.position, transform.rotation);

            gameObject.SetActive(false);
            if (healthBar != null)
            {
                healthBar.Reset();
                healthBar = null;
            }

            if (!keepUpgrades)
            {
                model.ResetConfig();
            }

            float value = model.Config.Cost / model.GeneralConfig.EnemyKillRewardMultipleByCost;
            int roundedValue = Mathf.RoundToInt(value / 5.0f) * 5;
            StatsManager.Instance.Money += roundedValue;
        }

        public void Freeze()
        {
            model.UpdateIsMoving(false);
        }

        public void UnFreeze()
        {
            model.UpdateIsMoving(true);
        }

        public void Knockback(Vector3 force)
        {
            rigidBody.AddForce(force, ForceMode.Impulse);
        }
        #endregion

        #region Private Functions
        private bool isSurvivalPhase()
        {
            return PhaseManager.Instance.CurrentPhase == PhaseManager.Phase.Survival;
        }

        private bool isPrepPhase()
        {
            return PhaseManager.Instance.CurrentPhase == PhaseManager.Phase.Prep;
        }

        private void configureAgent()
        {
            agent.speed = model.Config.MovementSpeed;
            agent.acceleration = model.Config.MovementAcceleration;
            agent.angularSpeed = model.Config.AngularSpeed;
            agent.radius = model.Config.NavMeshAgentRadius;
            agent.height = model.Config.NavMeshAgentHeight;
            agent.avoidancePriority = model.Config.NavMeshAgentPriority;
        }

        private bool isFrameReady()
        {
            return Time.frameCount % frameReadyInterval == 0;
        }

        private void updateInRangeTarget()
        {
            GameObject[] towers = GameObject.FindGameObjectsWithTag(model.TargetTag)
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

            bool isClosestTarget = nearestTarget != null && shortestDistance <= model.Config.AttackRange;
            if (isClosestTarget)
            {
                inRangeTargetTransform = nearestTarget.transform;
                if (inRangeTarget == null)
                {
                    inRangeTarget = inRangeTargetTransform.GetComponent<IDamageable>();
                }
                return;
            }

            inRangeTargetTransform = null;
            inRangeTarget = null;

            if (agent.enabled && agent.isOnNavMesh) 
            {
                model.UpdateIsMoving(true);
            }
        }

        private void updateUltimateTarget()
        {
            GameObject[] gotchis = GameObject.FindGameObjectsWithTag(model.TargetTag)
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

        private void rotateTowardInRangeTarget()
        {
            model.UpdateIsMoving(false);

            Vector3 dir = inRangeTargetTransform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * model.Config.AttackRotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        private void attackInRangeTarget()
        {
            bool isAttacking = attackCountdownTracker > 0f;
            if (isAttacking)
            {
                attackCountdownTracker -= Time.deltaTime;
                return;
            }

            attackCountdownTracker = model.Config.AttackCountdown;

            if(attackAnimation != null) attackAnimation.SetTrigger(model.AttackAnimTriggerHash);
            if(attackParticleEffect != null) attackParticleEffect.SetActive(true);
            EventBus.EnemyEvents.EnemyAttacked(model.Config.Type);
            inRangeTarget.Damage(model.Config.AttackDamage);
        }

        private void handleOnMovementSpeedUpdated()
        {
            agent.speed = model.MovementSpeed;
        }

        private void handleOnHealthUpdated()
        {
            float damage = healthBar.CurrentHealth - model.Health;
            healthBar.ShowDamagePopUpAndColorDifferentlyIfEnemy(damage, true);
            healthBar.CurrentHealth = model.Health;

            if (model.Health <= 0)
            {
                PlayDead(true);
                StatsManager.Instance.TrackKillEnemy(model.EnemyBlueprint);
            }
        }

        private void handleIsMovingUpdated()
        {
            agent.isStopped = !model.IsMoving;
        }
        #endregion
    }
}