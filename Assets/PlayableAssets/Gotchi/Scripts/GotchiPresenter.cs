using System.Collections.Generic;
using System.Linq;
using Fusion;
using GameMaster;
using Gotchi.Lickquidator.Manager;
using Gotchi.Lickquidator.Presenter;
using Gotchi.Network;
using Gotchi.Player.Model;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PhaseManager;
using PhaseManager.Presenter;

namespace Gotchi.Player.Presenter
{
    public class GotchiPresenter : NetworkBehaviour, IPlayerLeft
    {
        #region Properties
        public GotchiModel Model { get { return model; } }
        #endregion

        #region Fields
        [Header("Model")]
        [SerializeField] private GotchiModel model = null;

        [Header("View")]
        [SerializeField] private RangeCircle rangeCircle = null;
        [SerializeField] private ImpactPool_FX.ImpactType deathEffect = ImpactPool_FX.ImpactType.BasicTower;
        [SerializeField] private Animator attackAnimation = null;
        [SerializeField] private GameObject attackParticleEffect = null;
        [SerializeField] private Animator spinAttackAnimation = null;
        [SerializeField] private GameObject spinAttackParticleEffect = null;

        [Header("Gameplay")]
        [SerializeField] private InputActionAsset inputActions = null;
        [SerializeField] private string actionMapKey = "Player";
        [SerializeField] private string rightClickKey = "RightClick";
        [SerializeField] private int frameReadyInterval = 10;
        #endregion

        #region Private Variables
        private InputActionMap actionMap = null;
        private InputAction rightClick = null;
        private NavMeshAgent agent = null;
        private Camera mainCam = null;
        private HealthBar_UI healthBar = null;
        private Transform inRangeTargetTransform = null;
        private LickquidatorPresenter inRangeTarget = null;
        private float attackCountdownTracker = 0.5f;
        #endregion

        #region Unity Functions
        void Awake()
        {
            mainCam = Camera.main;
            agent = GetComponent<NavMeshAgent>();
            actionMap = inputActions.FindActionMap(actionMapKey);
            rightClick = actionMap.FindAction(rightClickKey);
            rangeCircle.SetScale(model.Config.SpinAbilityRange);
        }

        void Start()
        {
            configureAgent();
        }

        void OnEnable()
        {
            model.OnDestinationUpdated += handleOnDestinationUpdated;
            model.OnHealthUpdated += handleOnHealthUpdated;
            model.OnUsernameUpdated += handleOnUsernameUpdated;
            model.OnIsSpinAttackingUpdated += handleOnIsSpinAttackingUpdated;

            rightClick.performed += handleOnRightClick;
            rightClick.Enable();
        }

        void OnDisable()
        {
            model.OnDestinationUpdated -= handleOnDestinationUpdated;
            model.OnHealthUpdated -= handleOnHealthUpdated;
            model.OnUsernameUpdated -= handleOnUsernameUpdated;
            model.OnIsSpinAttackingUpdated -= handleOnIsSpinAttackingUpdated;

            rightClick.performed -= handleOnRightClick;
            rightClick.Disable();
        }

        void OnDestroy() 
        {
            GotchiManager.Instance.RemoveGotchi(this);
        }

        void Update()
        {
            if (isTransitionPhase()) return;

            if (isFrameReady())
            {
                updateInRangeTarget();
            }

            if (inRangeTarget == null) return;

            rotateTowardInRangeTarget();
            attackInRangeTarget();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = model.RangeIndicatorColor;
            Gizmos.DrawWireSphere(transform.position, model.Config.AttackRange);
        }
        #endregion

        #region Networked Functions
        public override void Spawned()
        {
            model.Username = PlayerPrefs.GetString("username");

            if (Object.HasInputAuthority)
            {
                Debug.Log("Spawned local player");
                NetworkManager.Instance.LocalPlayerGotchi = this;
                assignHealthBar();
            }
            else
            {
                Debug.Log("Spawned remote player");
            }
            GotchiManager.Instance.RegisterGotchi(this);

        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
            {
                Debug.Log("Local player left, despawning");
                Runner.Despawn(Object);
                SceneManager.LoadScene("GotchiTowerDefense-WithTerrain");
            }
            else
            {
                Debug.Log("Remote player left");
                UserInterfaceManager.Instance.PlayersListUI.RemovePlayerEntry(model.Username);
            }
        }
        #endregion

        #region Public Functions
        public void SpinAttack()
        {
            HandleOnExitAbilityButton();
            model.IsSpinAttacking = true;
        }

        public void Damage(int damage)
        {
            GameMasterEvents.GotchiEvents.GotchiDamaged(gameObject.GetInstanceID(), damage);
            model.UpdateHealth(model.Health - damage);
        }

        // TODO: refactor AbilityButton_UI to its own MVP component 
        public void HandleOnHoverAbilityButton()
        {
            rangeCircle.ToggleActive(true);
        }

        public void HandleOnExitAbilityButton()
        {
            rangeCircle.ToggleActive(false);
        }

        public bool IsDead()
        {
            return model.Health <= 0;
        }
        #endregion

        #region Event Handlers
        private void handleOnDestinationUpdated()
        {
            agent.velocity = Vector3.zero;
            agent.SetDestination(model.Destination);
        }

        private void handleOnUsernameUpdated()
        {
            UserInterfaceManager.Instance.PlayersListUI.AddPlayerEntry(gameObject.GetInstanceID(), model.Username, Object.HasInputAuthority);
        }

        private void handleOnRightClick(InputAction.CallbackContext Context)
        {
            if (!Context.performed || isTransitionPhase()) return;

            Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                NavMeshPath path = new NavMeshPath();
                bool canMoveToTarget = agent.CalculatePath(hit.point, path) && path.status == NavMeshPathStatus.PathComplete;
                
                if (canMoveToTarget)
                {
                    model.Destination = hit.point;
                }

                GotchiWaypointsPool_UI.Instance.ShowCanMovePopUp(hit.point, canMoveToTarget);
            }
        }

        private void handleOnIsSpinAttackingUpdated()
        {
            if (!model.IsSpinAttacking) return;

            if (spinAttackAnimation != null) spinAttackAnimation.SetTrigger(model.AbilityAnimTriggerHash);
            if (spinAttackParticleEffect != null) spinAttackParticleEffect.SetActive(true);

            GameMasterEvents.GotchiEvents.GotchiAttacked(gameObject.GetInstanceID(), GotchiManager.AttackType.Spin);

            if (isPrepPhase()) 
            {
                model.IsSpinAttacking = false;
                return;
            }

            // TODO: instead of networking it this way, it probably would be better to just network the lickquidators' transforms and healths
            List<LickquidatorPresenter> lickquidators = LickquidatorManager.Instance.ActiveLickquidators;
            foreach (LickquidatorPresenter lickquidator in lickquidators)
            {
                float distanceToTarget = Vector3.Distance(transform.position, lickquidator.transform.position);
                bool isInRange = distanceToTarget < model.Config.SpinAbilityRange;
                if (!isInRange) break;
                
                lickquidator.Damage(model.Config.SpinAbilityDamage);
                Vector3 direction = (lickquidator.transform.position - transform.position).normalized;
                lickquidator.Knockback(direction * model.Config.SpinAbilityKnockbackForce);
            }

            model.IsSpinAttacking = false;
        }

        private void handleOnHealthUpdated()
        {
            if (healthBar.CurrentHealth <= 0) return;

            float damage = healthBar.CurrentHealth - model.Health;
            healthBar.ShowDamagePopUpAndColorDifferentlyIfEnemy(damage, true);
            healthBar.CurrentHealth = model.Health;

            if (model.Health > 0) return;
            
            playDead();
        }
        #endregion

        #region Private Functions
        // TODO: these can probably go into the phase manager or as helpers
        private bool isTransitionPhase()
        {
            return PhasePresenter.Instance.GetCurrentPhase() != Phase.Prep
                && PhasePresenter.Instance.GetCurrentPhase() != Phase.Survival;
        }

        private bool isPrepPhase()
        {
            return PhasePresenter.Instance.GetCurrentPhase() == Phase.Prep;
        }

        private void configureAgent()
        {
            agent.Warp(GotchiManager.Instance.Spawn.position);
            agent.speed = model.Config.MovementSpeed;
            agent.acceleration = model.Config.MovementAcceleration;
            agent.angularSpeed = model.Config.AngularSpeed;
            agent.radius = model.Config.NavMeshAgentRadius;
            agent.height = model.Config.NavMeshAgentHeight;
            agent.avoidancePriority = model.Config.NavMeshAgentPriority;
        }

        private void assignHealthBar()
        {
            healthBar = HealthBarPool_UI.Instance.GetHealthbar(model.HealthBarOffset);
            healthBar.SetHealthbarMaxHealth(model.Health);
        }

        private void updateInRangeTarget()
        {
            GameObject[] lickquidators = GameObject.FindGameObjectsWithTag(model.TargetTag)
                .Where(lickquidator => lickquidator.activeSelf).ToArray();
            GameObject nearestTarget = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject lickquidator in lickquidators)
            {
                float distanceToTarget = Vector3.Distance(transform.position, lickquidator.transform.position);
                bool isCloserTarget = distanceToTarget < shortestDistance;
                if (isCloserTarget)
                {
                    shortestDistance = distanceToTarget;
                    nearestTarget = lickquidator;
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
                inRangeTarget = inRangeTargetTransform.GetComponent<LickquidatorPresenter>();
                attackCountdownTracker = model.Config.AttackCountdown;
                return;
            }

            inRangeTargetTransform = null;
            inRangeTarget = null;
        }

        private void rotateTowardInRangeTarget()
        {
            Vector3 dir = inRangeTargetTransform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * model.Config.AttackRotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        private void attackInRangeTarget()
        {
            if (attackCountdownTracker > 0f)
            {
                attackCountdownTracker -= Time.deltaTime;
                return;
            }

            attackCountdownTracker = model.Config.AttackCountdown;

            if (attackAnimation != null) attackAnimation.SetTrigger(model.AttackAnimTriggerHash);
            if (attackParticleEffect != null) attackParticleEffect.SetActive(true);

            GameMasterEvents.GotchiEvents.GotchiAttacked(gameObject.GetInstanceID(), GotchiManager.AttackType.Basic);
            inRangeTarget.Damage(model.Config.AttackDamage);
        }

        private void playDead()
        {
            GameMasterEvents.GotchiEvents.GotchiDied(gameObject.GetInstanceID());
            ImpactPool_FX.Instance.SpawnImpact(deathEffect, transform.position, transform.rotation);
            
            gameObject.SetActive(false);
            if (healthBar != null)
            {
                healthBar.Reset();
                healthBar = null;
            }
        }

        private bool isFrameReady()
        {
            return Time.frameCount % frameReadyInterval == 0;
        }
        #endregion
    }
}
