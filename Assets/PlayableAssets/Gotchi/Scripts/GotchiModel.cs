using System;
using Fusion;
using UnityEngine;

namespace Gotchi.Player.Model
{
    public class GotchiModel : NetworkBehaviour
    {
        #region Events
        public event Action OnHealthUpdated = delegate { };
        public event Action OnDestinationUpdated = delegate { };
        public event Action OnUsernameUpdated = delegate { };
        public event Action OnIsSpinAttackingUpdated = delegate { };
        #endregion

        #region Properties
        public GotchiObjectSO Config { get { return config; } }
        public int Health { get { return health; } }
        public string TargetTag { get { return targetTag; } }
        public int AttackAnimTriggerHash { get { return attackAnimTriggerHash; } }
        public int AbilityAnimTriggerHash { get { return abilityAnimTriggerHash; } }
        public Color RangeIndicatorColor { get { return rangeIndicatorColor; } }

        public Transform HealthBarOffset { get { return healthBarOffset; } }
        #endregion

        #region Networked Properties
        [Networked(OnChanged = nameof(Network_HandleOnDestinationUpdated))]
        public Vector3 Destination { get; set; }
        [Networked(OnChanged = nameof(Network_HandleOnUsernameUpdated))]
        public string Username { get; set; }
        [Networked(OnChanged = nameof(Network_HandleOnIsSpinAttackingUpdated))]
        public bool IsSpinAttacking { get; set; }
        #endregion

        #region Fields
        [SerializeField] private GotchiObjectSO config = null;
        [SerializeField] private string targetTag = "Enemy";
        [SerializeField] private string attackAnimTrigger = "Swing";
        [SerializeField] private string abilityAnimTrigger = "Spin";
        [SerializeField] private Color rangeIndicatorColor = Color.red;

        [SerializeField] private Transform healthBarOffset = null;
        #endregion

        #region Private Variables
        private int health = 0;
        private int attackAnimTriggerHash = 0;
        private int abilityAnimTriggerHash = 0;
        #endregion
    
        #region Unity Functions
        void Awake()
        {
            health = config.Health;
            attackAnimTriggerHash = Animator.StringToHash(attackAnimTrigger);
            abilityAnimTriggerHash = Animator.StringToHash(abilityAnimTrigger);
        }
        #endregion

        #region Networked Functions
        public static void Network_HandleOnDestinationUpdated(Changed<GotchiModel> model)
        {
            model.Behaviour.OnDestinationUpdated();
        }

        public static void Network_HandleOnUsernameUpdated(Changed<GotchiModel> model)
        {
            model.Behaviour.OnUsernameUpdated();
        }

        public static void Network_HandleOnIsSpinAttackingUpdated(Changed<GotchiModel> model)
        {
            model.Behaviour.OnIsSpinAttackingUpdated();
        }
        #endregion

        #region Public Functions
        public void UpdateHealth(int health)
        {
            this.health = health;
            OnHealthUpdated();
        }
        #endregion
    }
}
