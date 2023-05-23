using UnityEngine;
using System;

namespace Gotchi.Lickquidator.Model
{
    public abstract class LickquidatorModel : MonoBehaviour
    {
        #region Events
        public event Action OnMovementSpeedUpdated = delegate {};
        public event Action OnHealthUpdated = delegate {};
        // public event Action OnIsMovingUpdated = delegate {};
        public event Action OnAttacked = delegate {};
        #endregion

        #region Properties
        public EnemyBlueprint EnemyBlueprint { get; set; }
        public LickquidatorObjectSO Config { get { return config; } }
        public GeneralSO GeneralConfig { get { return generalConfig; } }
        public float MovementSpeed { get { return movementSpeed; }}
        public int Health { get { return health; } }
        public int MaxHealth { get { return maxHealth; } }
        public string TargetTag { get { return targetTag; } }
        public int AttackAnimTriggerHash { get { return attackAnimTriggerHash; } }
        public Color RangeIndicatorColor { get { return rangeIndicatorColor; } }
        public Transform HealthBarOffset { get { return healthBarOffset; } }
        public bool IsPassive { get { return isPassive; } }
        public bool IsRanged { get { return isRanged; } }
        #endregion
        
        #region Fields
        [SerializeField] private LickquidatorObjectSO origConfig = null;
        [SerializeField] private GeneralSO generalConfig = null;
        [SerializeField] private string targetTag = "Tower";
        [SerializeField] private string attackAnimTrigger = "Swipe";
        [SerializeField] private Color rangeIndicatorColor = Color.red;
        [SerializeField] private Transform healthBarOffset = null;
        [SerializeField] private bool isPassive = false;
        [SerializeField] private bool isRanged = true;
        #endregion
        
        #region Private Variables
        private LickquidatorObjectSO config = null;
        private float movementSpeed = 0f;
        private int health = 0;
        private int maxHealth = 0;
        private int attackAnimTriggerHash = 0;
        #endregion

        #region Unity Functions
        protected virtual void Awake()
        {
            movementSpeed = origConfig.MovementSpeed;
            health = origConfig.Health;
            maxHealth = origConfig.Health;
            config = ScriptableObject.CreateInstance<LickquidatorObjectSO>();
            ResetConfig();
            attackAnimTriggerHash = Animator.StringToHash(attackAnimTrigger);
        }
        #endregion

        #region Public Functions
        public void UpdateMovementSpeed(float movementSpeed)
        {
            if (this.movementSpeed == movementSpeed) return;
            this.movementSpeed = movementSpeed; 
            OnMovementSpeedUpdated();
        }

        public void UpdateHealth(int health)
        {
            if (this.health == health) return;
            this.health = health;
            OnHealthUpdated();
        }

        public void PublishOnAttacked()
        {
            OnAttacked();
        }

        public void ResetConfig()
        {
            config.Name = origConfig.Name;
            config.Type = origConfig.Type;
            config.Level = origConfig.Level;
            config.AttackDamage = origConfig.AttackDamage;
            config.AttackRange = origConfig.AttackRange;
            config.AttackCountdown = origConfig.AttackCountdown;
            config.Cost = origConfig.Cost;
            config.buildTime = origConfig.buildTime;
            config.Health = origConfig.Health;
            config.OffsetDistance = origConfig.OffsetDistance;
            config.MovementSpeed = origConfig.MovementSpeed;
            config.MovementAcceleration = origConfig.MovementAcceleration;
            config.AngularSpeed = origConfig.AngularSpeed;
            config.AttackRotationSpeed = origConfig.AttackRotationSpeed;
            config.NavMeshAgentHeight = origConfig.NavMeshAgentHeight;
            config.NavMeshAgentPriority = origConfig.NavMeshAgentPriority;
            config.NavMeshAgentRadius = origConfig.NavMeshAgentRadius;
            config.NavMeshAgentStoppingDistance = origConfig.NavMeshAgentStoppingDistance;
        }
        #endregion
    }
}