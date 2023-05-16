using UnityEngine;
using System;

namespace Gotchi.Lickquidators
{
    public class LickquidatorModel : MonoBehaviour
    {
        public event Action OnMovementSpeedUpdated = delegate {};
        public event Action OnHealthUpdated = delegate {};
        public event Action OnIsMovingUpdated = delegate {};

        public EnemyBlueprint EnemyBlueprint { get; set; }
        public LickquidatorObjectSO Config { get { return config; } }
        public GeneralSO GeneralConfig { get { return generalConfig; } }
        public float MovementSpeed { get { return movementSpeed; }}
        public int Health { get { return health; }}
        public bool IsMoving { get { return isMoving; }}
        public string TargetTag { get { return targetTag; } }
        public int AttackAnimTriggerHash { get { return attackAnimTriggerHash; } }
        public Color RangeIndicatorColor { get { return rangeIndicatorColor; } }
        public Transform HealthBarOffset { get { return healthBarOffset; } }
        public bool IsPassive { get { return isPassive; } }
        
        [SerializeField] private LickquidatorObjectSO origConfig = null;
        [SerializeField] private GeneralSO generalConfig = null;
        [SerializeField] private string targetTag = "Tower";
        [SerializeField] private string attackAnimTrigger = "Swipe";
        [SerializeField] private Color rangeIndicatorColor = Color.red;
        [SerializeField] private Transform healthBarOffset = null;
        [SerializeField] private bool isPassive = false;
        
        private int attackAnimTriggerHash = 0;
        private LickquidatorObjectSO config = null;
        private float movementSpeed = 0f;
        private int health = 0;
        private bool isMoving = false;

        void Awake()
        {
            movementSpeed = origConfig.MovementSpeed;
            health = origConfig.Health;
            config = ScriptableObject.CreateInstance<LickquidatorObjectSO>();
            ResetConfig();
            attackAnimTriggerHash = Animator.StringToHash(attackAnimTrigger);
        }

        public void UpdateMovementSpeed(float movementSpeed)
        {
            this.movementSpeed = movementSpeed; 
            OnMovementSpeedUpdated();
        }

        public void UpdateHealth(int health)
        {
            this.health = health;
            OnHealthUpdated();
        }

        public void UpdateIsMoving(bool isMoving)
        {
            this.isMoving = isMoving;
            OnIsMovingUpdated();
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
    }
}