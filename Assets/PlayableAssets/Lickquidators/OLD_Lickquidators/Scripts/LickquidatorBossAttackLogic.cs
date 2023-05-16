using UnityEngine;
using System.Collections;
using Gotchi.Events;
using Gotchi.New;

public class LickquidatorBossAttackLogic : BaseEnemy
{
    #region Fields
    [Header("Tongue Swipe Specific")]
    [SerializeField] private GameObject salivaEffect = null;
    #endregion

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();
        anim = transform.Find("TongueSwipe").Find("Tongue").GetComponent<Animator>();
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
        EventBus.EnemyEvents.EnemyAttacked(LickquidatorManager.LickquidatorType.BossLickquidator);
    }
    #endregion
}
