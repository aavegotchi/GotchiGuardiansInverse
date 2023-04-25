using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/General")]
public class GeneralSO : ScriptableObject
{
    [Header("Economy")]
    public int SpiritGems = 500;
    public float EnemyKillRewardMultipleByCost = 0.5f;
    public float EnemySpawnRewardMultipleByCost = 0.25f;
    public float TowerSellRewardMultipleByCost = 0.5f;
    public float EnemySellRewardMultipleByCost = 0.5f;
    public float GenericUpgradeMultipleByLevel = 2f;

    [Header("Phases")]
    public float PrepPhaseCountdown = 30f;
}
