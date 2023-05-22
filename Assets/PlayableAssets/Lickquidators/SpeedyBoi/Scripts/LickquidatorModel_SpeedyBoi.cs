using UnityEngine;
using Gotchi.Lickquidator.Model;

namespace Gotchi.Lickquidator.SpeedyBoi.Model
{
    public class LickquidatorModel_SpeedyBoi : LickquidatorModel
    {
        #region Properties
        public int MoveAnimTriggerHash { get { return moveAnimTriggerHash; } }
        public int KnockbackAnimTriggerHash { get { return knockbackAnimTriggerHash; } }
        #endregion

        #region Fields
        [SerializeField] private string moveAnimTrigger = "Roll";
        [SerializeField] private string knockbackAnimTrigger = "Knockback";
        #endregion

        #region Private Varibles
        private int moveAnimTriggerHash = 0;
        private int knockbackAnimTriggerHash = 0;
        #endregion

        #region Unity Functions
        protected override void Awake()
        {
            base.Awake();
            moveAnimTriggerHash = Animator.StringToHash(moveAnimTrigger);
            knockbackAnimTriggerHash = Animator.StringToHash(knockbackAnimTrigger);
        }
        #endregion  
    }
}