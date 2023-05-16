using System.Collections;
using UnityEngine;
using Gotchi.Events;
using Gotchi.New;

public class LickquidatorPawnAttackLogic : BaseEnemy
{
    #region Fields
    [Header("Tongue Swipe Specific")]
    [SerializeField] private GameObject salivaEffect = null;
    #endregion

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();
        //anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        attackParticleEffectGO = salivaEffect;
        attackTrigger = "Swipe";
    }
    #endregion

    #region Protected Functions
    protected override IEnumerator Attack()
    {
        return base.Attack();
    }

    protected override void OnAttackSound()
    {
        EventBus.EnemyEvents.EnemyAttacked(LickquidatorManager.LickquidatorType.PawnLickquidator);
    }
    #endregion
}
