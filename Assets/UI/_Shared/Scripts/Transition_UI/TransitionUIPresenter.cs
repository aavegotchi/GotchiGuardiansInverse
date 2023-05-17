using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TransitionUI.Model;

namespace TransitionUI {
    namespace Presenter
    {
        public class TransitionUIPresenter : MonoBehaviour
        {
            #region Public Variables
            #endregion

            #region Fields
            [Header("Required Refs")]
            [SerializeField] private TextMeshProUGUI transitionCountdownTextUI = null;
            [SerializeField] private GameObject rewardsScreenUI = null;
            [SerializeField] private TextMeshProUGUI pawnLickquidatorRewardTextUI = null;
            [SerializeField] private TextMeshProUGUI aerialLickquidatorRewardTextUI = null;
            [SerializeField] private TextMeshProUGUI bossLickquidatorRewardTextUI = null;
            [SerializeField] private TextMeshProUGUI basicTowerRewardTextUI = null;
            [SerializeField] private TextMeshProUGUI arrowTowerRewardTextUI = null;
            [SerializeField] private TextMeshProUGUI fireTowerRewardTextUI = null;
            [SerializeField] private TextMeshProUGUI iceTowerRewardTextUI = null;

            [SerializeField] private TextMeshProUGUI pawnLickquidatorCostTextUI = null;
            [SerializeField] private TextMeshProUGUI aerialLickquidatorCostTextUI = null;
            [SerializeField] private TextMeshProUGUI bossLickquidatorCostTextUI = null;
            [SerializeField] private TextMeshProUGUI basicTowerCostTextUI = null;
            [SerializeField] private TextMeshProUGUI arrowTowerCostTextUI = null;
            [SerializeField] private TextMeshProUGUI fireTowerCostTextUI = null;
            [SerializeField] private TextMeshProUGUI iceTowerCostTextUI = null;
            [SerializeField] private TextMeshProUGUI enemiesSpawnBonusTextUI = null;
            [SerializeField] private TextMeshProUGUI netTextUI = null;
            #endregion

            #region Private Variables
            private TransitionUIModel TransitionUIModel;

            private Animator _transitionScreenAnimator = null;
            #endregion

            #region Unity Functions
            private void Start()
            {
                TransitionUIModel = new TransitionUIModel();
                TransitionUIModel.ShowRewardsUIUpdated += HandleShowRewardsUIUpdate;
                TransitionUIModel.UpdateTransitionText += HandleUpdateTransitionCountdownText;
                TransitionUIModel.UpdateShowTransitionUI += HandleShowTransitionUIUpdate;
                _transitionScreenAnimator = GetComponent<Animator>();
            }

            #endregion

            #region Private Functions
            private void HandleShowRewardsUIUpdate(bool isOpen)
            {
                if (isOpen) {
                    ShowRewardsUI();
                } else {
                    HideRewardsUI();
                }
            }

            private void HandleShowTransitionUIUpdate(bool isOpen) 
            {
                if (isOpen) {
                    _transitionScreenAnimator.SetTrigger("Open");
                } else {
                    _transitionScreenAnimator.SetTrigger("Close");
                }
            }

            private void HandleUpdateTransitionCountdownText(string text)
            {
                transitionCountdownTextUI.text = text;
            }

            private void ShowRewardsUI() 
            {
                rewardsScreenUI.SetActive(true);

                int pawnLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(EnemyPool.EnemyType.PawnLickquidator);
                int aerialLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(EnemyPool.EnemyType.AerialLickquidator);
                int bossLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(EnemyPool.EnemyType.BossLickquidator);

                // TODO: account for upgraded towers
                int basicTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.BasicTower);
                int arrowTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.ArrowTower1);
                int fireTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.FireTower1);
                int iceTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.IceTower1);

                int pawnLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(EnemyPool.EnemyType.PawnLickquidator);
                int aerialLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(EnemyPool.EnemyType.AerialLickquidator);
                int bossLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(EnemyPool.EnemyType.BossLickquidator);

                // TODO: account for upgraded towers
                int basicTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerPool.TowerType.BasicTower);
                int arrowTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerPool.TowerType.ArrowTower1);
                int fireTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerPool.TowerType.FireTower1);
                int iceTowerCreateCosts = StatsManager.Instance.GetTowerCreateCosts(TowerPool.TowerType.IceTower1);

                int enemiesSpawnReward = StatsManager.Instance.GetEnemiesSpawnBonus();

                pawnLickquidatorRewardTextUI.text = $"${pawnLickquidatorKillCosts}";
                aerialLickquidatorRewardTextUI.text = $"${aerialLickquidatorKillCosts}";
                bossLickquidatorRewardTextUI.text = $"${bossLickquidatorKillCosts}";

                basicTowerRewardTextUI.text = $"${basicTowerKillCosts}";
                arrowTowerRewardTextUI.text = $"${arrowTowerKillCosts}";
                fireTowerRewardTextUI.text = $"${fireTowerKillCosts}";
                iceTowerRewardTextUI.text = $"${iceTowerKillCosts}";

                pawnLickquidatorCostTextUI.text = $"${pawnLickquidatorCreateCosts}";
                aerialLickquidatorCostTextUI.text = $"${aerialLickquidatorCreateCosts}";
                bossLickquidatorCostTextUI.text = $"${bossLickquidatorCreateCosts}";

                basicTowerCostTextUI.text = $"${basicTowerCreateCosts}";
                arrowTowerCostTextUI.text = $"${arrowTowerCreateCosts}";
                fireTowerCostTextUI.text = $"${fireTowerCreateCosts}";
                iceTowerCostTextUI.text = $"${iceTowerCreateCosts}";

                enemiesSpawnBonusTextUI.text = $"${enemiesSpawnReward}";

                int net = pawnLickquidatorKillCosts + aerialLickquidatorKillCosts + bossLickquidatorKillCosts
                    + basicTowerKillCosts + arrowTowerKillCosts + fireTowerKillCosts + iceTowerKillCosts
                    - pawnLickquidatorCreateCosts - aerialLickquidatorCreateCosts - bossLickquidatorCreateCosts
                    - basicTowerCreateCosts - arrowTowerCreateCosts - fireTowerCreateCosts - iceTowerCreateCosts
                    + enemiesSpawnReward;

                netTextUI.text = $"{net}";
            }

            private void HideRewardsUI()
            {

                rewardsScreenUI.SetActive(false);
            }
            #endregion
        }
    }
}