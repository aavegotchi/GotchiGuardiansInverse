using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Transition_UI.Model;
using Gotchi.Lickquidators;

namespace Transition_UI.Presenter {
    public class Transition_UI_Presenter : MonoBehaviour
    {
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

        #region Public Variables
        public Transition_UI_Model Model { get {return model;} }
        #endregion

        #region Private Variables
        private Transition_UI_Model model;

        private Animator _transitionScreenAnimator = null;
        #endregion

        #region Unity Functions
        private void Start()
        {
            model = new Transition_UI_Model();
            model.OnShowTransitionUIUpdated += HandleShowTransitionUIUpdate;
            model.OnTransitionUITextUpdated += HandleUpdateTransitionCountdownText;
            model.OnIsRewardsUIOpenUpdated += HandleShowRewardsUIUpdate;
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

            int pawnLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(LickquidatorManager.LickquidatorType.PawnLickquidator);
            int aerialLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(LickquidatorManager.LickquidatorType.AerialLickquidator);
            int bossLickquidatorKillCosts = StatsManager.Instance.GetEnemyKillCosts(LickquidatorManager.LickquidatorType.BossLickquidator);

            // TODO: account for upgraded towers
            int basicTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.BasicTower);
            int arrowTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.ArrowTower1);
            int fireTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.FireTower1);
            int iceTowerKillCosts = StatsManager.Instance.GetTowerKillCosts(TowerPool.TowerType.IceTower1);

            int pawnLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(LickquidatorManager.LickquidatorType.PawnLickquidator);
            int aerialLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(LickquidatorManager.LickquidatorType.AerialLickquidator);
            int bossLickquidatorCreateCosts = StatsManager.Instance.GetEnemyCreateCosts(LickquidatorManager.LickquidatorType.BossLickquidator);

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