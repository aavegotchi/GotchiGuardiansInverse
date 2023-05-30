using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using PhaseManager;
using PhaseManager.Presenter;
using Gotchi.Lickquidator.Model;
using Gotchi.Player.Presenter;
using Gotchi.Lickquidator.Splitter.Model;
using Gotchi.Lickquidator.Splitter.Presenter;
using GameMaster;
using Gotchi.Lickquidator.Manager;

namespace Gotchi.Lickquidator.Presenter
{
    public abstract class LickquidatorPresenter : MonoBehaviour
    {
        #region Properties
        public LickquidatorModel Model { get { return model; } }
        #endregion

        #region Fields
        [Header("Model")]
        [SerializeField] protected LickquidatorModel model = null;

        [Header("View")]
        [SerializeField] private ImpactPool_FX.ImpactType deathEffect = ImpactPool_FX.ImpactType.BasicTower;
        [SerializeField] private Animator attackAnimation = null;
        [SerializeField] private GameObject attackParticleSystemObj = null;

        [Header("Gameplay")]
        [SerializeField] private int frameReadyInterval = 10;
        #endregion

        #region Private Variables
        private HealthBar_UI healthBar = null;
        protected NavMeshAgent agent;
        private Transform inRangeTargetTransform = null;
        private GotchiPresenter inRangeTarget = null;
        protected Rigidbody rigidBody = null;
        private float attackCountdownTracker = 0.5f;
        #endregion

        #region Unity Functions
        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            configureAgent();
        }

        protected virtual void OnEnable()
        {
            model.OnMovementSpeedUpdated += handleOnMovementSpeedUpdated;
            model.OnHealthUpdated += handleOnHealthUpdated;
        }

        protected virtual void OnDisable()
        {
            model.OnMovementSpeedUpdated -= handleOnMovementSpeedUpdated;
            model.OnHealthUpdated -= handleOnHealthUpdated;
        }

        void Update()
        {
            if (!isSurvivalPhase()) return;

            if (isFrameReady())
            {
                if (model.IsRanged)
                {
                    updateInRangeTarget();
                }
                updateUltimateTarget();
            }

            if (model.IsPassive || inRangeTarget == null) return;

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
            if (model.Config == null) return;

            Gizmos.color = model.RangeIndicatorColor;
            //Gizmos.DrawWireSphere(transform.position, model.Config.AttackRange);
        }

        void OnCollisionEnter(Collision other)
        {
            if (model.IsRanged || other.gameObject.tag != model.TargetTag) return;

            inRangeTargetTransform = other.gameObject.transform;
            inRangeTarget = inRangeTargetTransform.GetComponent<GotchiPresenter>();
        }
        #endregion

        #region Public Functions
        public void Damage(int damage)
        {
            model.UpdateHealth(model.Health - damage);
        }

        public void AssignHealthBar()
        {
            healthBar = HealthBarPool_UI.Instance.GetHealthbar(model.HealthBarOffset);

            model.UpdateHealth(model.MaxHealth);

            healthBar.SetHealthbarMaxHealth(model.Health);
        }

        public void PlayDead(bool keepUpgrades = false)
        {
            if (model is LickquidatorModel_Splitter && model.GetComponent<LickquidatorPresenter_Splitter>().IsGoingToSplitOnDeath())
            {
                HandleSplitter(keepUpgrades);
            } else
            {
                HandleNormalDeath(keepUpgrades);
            }
        }

        private void HandleSplitter(bool keepUpgrades)
        {
            GameMasterEvents.EnemyEvents.EnemyDied(model.EnemyBlueprint.type);
            ImpactPool_FX.Instance.SpawnImpact(deathEffect, transform.position, transform.rotation);

            if (healthBar != null)
            {
                healthBar.Reset();
                healthBar = null;
            }

            LickquidatorManager.Instance.DelayDeactivationForSplitter(this);

            gameObject.SetActive(false);
        }

        private void HandleNormalDeath (bool keepUpgrades)
        {
            Debug.Log("model.EnemyBlueprint.type - " + model.EnemyBlueprint.type);

            GameMasterEvents.EnemyEvents.EnemyDied(model.EnemyBlueprint.type);
            ImpactPool_FX.Instance.SpawnImpact(deathEffect, transform.position, transform.rotation);

            if (healthBar != null)
            {
                healthBar.Reset();
                healthBar = null;
            }

            if (!keepUpgrades)
            {
                model.ResetConfig();
            }

            rewardDeath();

            // Notify the LickquidatorManager that this Lickquidator has been deactivated
            LickquidatorManager.Instance.DeactivateLickquidator(this);

            gameObject.SetActive(false);
        }

        public void Knockback(Vector3 force)
        {
            rigidBody.AddForce(force, ForceMode.Impulse);
        }

        protected Vector3 GetDirectionToTarget()
        {
            return inRangeTargetTransform.position - transform.position;
        }
        #endregion

        #region Event Handlers
        private void handleOnMovementSpeedUpdated()
        {
            agent.speed = model.MovementSpeed;
        }

        private void handleOnHealthUpdated()
        {
            if (healthBar.CurrentHealth <= 0) return;

            float damage = healthBar.CurrentHealth - model.Health;
            healthBar.ShowDamagePopUpAndColorDifferentlyIfEnemy(damage, true);
            healthBar.CurrentHealth = model.Health;

            if (model.Health > 0) return;

            PlayDead(true);
        }
        #endregion

        #region Private Functions
        private bool isSurvivalPhase()
        {
            return PhasePresenter.Instance.GetCurrentPhase() == Phase.Survival;
        }

        private bool isPrepPhase()
        {
            return PhasePresenter.Instance.GetCurrentPhase() == Phase.Prep;
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
                if (nearestTarget != null && inRangeTargetTransform != null && GameObject.ReferenceEquals(nearestTarget, inRangeTargetTransform.gameObject))
                {
                    return;
                }

                inRangeTargetTransform = nearestTarget.transform;
                inRangeTarget = inRangeTargetTransform.GetComponent<GotchiPresenter>();
                attackCountdownTracker = model.Config.AttackCountdown;
                return;
            }

            inRangeTargetTransform = null;
            inRangeTarget = null;
        }

        private void updateUltimateTarget()
        {
            GameObject[] gotchis = GameObject.FindGameObjectsWithTag(model.TargetTag)
                .Where(gotchi => gotchi.activeSelf && gotchi.GetComponent<GotchiPresenter>() != null && !gotchi.GetComponent<GotchiPresenter>().IsDead()).ToArray();
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

            if (nearestTarget != null && inRangeTargetTransform != null && GameObject.ReferenceEquals(nearestTarget, inRangeTargetTransform.gameObject))
            {
                return;
            }

            if (agent.enabled && agent.isOnNavMesh && nearestTarget != null)
            {
                agent.SetDestination(nearestTarget.transform.position);
            }
        }

        private void rotateTowardInRangeTarget()
        {
            Vector3 dir = GetDirectionToTarget();
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * model.Config.AttackRotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        private void attackInRangeTarget()
        {
            if (model.IsRanged)
            {
                if (attackCountdownTracker > 0f)
                {
                    attackCountdownTracker -= Time.deltaTime;
                    return;
                }

                attackCountdownTracker = model.Config.AttackCountdown;
            }

            if (attackAnimation != null) attackAnimation.SetTrigger(model.AttackAnimTriggerHash);
            if (attackParticleSystemObj != null) attackParticleSystemObj.SetActive(true);

            GameMasterEvents.EnemyEvents.EnemyAttacked(model.Config.Type);
            inRangeTarget.Damage(model.Config.AttackDamage);
            model.PublishOnAttacked();

            inRangeTargetTransform = null;
            inRangeTarget = null;
        }

        private void rewardDeath()
        {
            StatsManager.Instance.TrackKillEnemy(model.EnemyBlueprint);

            float value = model.Config.Cost / model.GeneralConfig.EnemyKillRewardMultipleByCost;
            int roundedValue = Mathf.RoundToInt(value / 5.0f) * 5;
            StatsManager.Instance.Money += roundedValue;
        }
        #endregion
    }
}