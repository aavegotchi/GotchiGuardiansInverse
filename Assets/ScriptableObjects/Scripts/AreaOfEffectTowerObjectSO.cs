using Gotchi.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaOfEffectTower", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Tower/AreaOfEffectTower")]
public class AreaOfEffectTowerObjectSO : BaseTowerObjectSO
{
    [Header("Area of Effect")]
    public float AreaOfEffectRange = 40f;
    public float SlowStrength = 1.5f;

    [Header("Death Effect Type")]
    public ImpactPool_FX.ImpactType deathEffectType;
    public override ImpactPool_FX.ImpactType DeathEffectType => deathEffectType;
}
