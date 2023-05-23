using UnityEngine;
using Gotchi.Lickquidator.Model;

namespace Gotchi.Lickquidator.SpeedyBoi.Model
{
    public class LickquidatorModel_SpeedyBoi : LickquidatorModel
    {
        #region Properties
        public int MoveAnimTriggerHash { get { return moveAnimTriggerHash; } }
        #endregion

        #region Fields
        [SerializeField] private string moveAnimTrigger = "Roll";
        #endregion

        #region Private Varibles
        private int moveAnimTriggerHash = 0;
        #endregion

        #region Unity Functions
        protected override void Awake()
        {
            base.Awake();
            moveAnimTriggerHash = Animator.StringToHash(moveAnimTrigger);
        }
        #endregion  
    }
}