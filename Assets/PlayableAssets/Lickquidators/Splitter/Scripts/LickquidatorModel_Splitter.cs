using Gotchi.Lickquidator.Model;
using UnityEngine;

namespace Gotchi.Lickquidator.Splitter.Model
{
    public class LickquidatorModel_Splitter : LickquidatorModel
    {
        #region Properties
        public int MoveAnimTriggerHash { get { return moveAnimTriggerHash; } }
        #endregion

        #region Fields
        [SerializeField] private string moveAnimTrigger = "Walk";
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
