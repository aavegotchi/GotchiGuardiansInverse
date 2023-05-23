using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using PhaseManager;
using PhaseManager.Presenter;
using Gotchi.Events;
using Gotchi.Bot.Model;
using Random = UnityEngine.Random;

namespace Gotchi.Bot.Presenter {
    public class GotchiBotPresenter: MonoBehaviour
    {
        #region Properties
        public GotchiBotModel Model { get { return Model; } }
        #endregion

        #region Fields
        [Header("Model")]
        [SerializeField] private GotchiBotModel model = null;
        #endregion

        #region Unity Functions
        void Awake()
        {
            model = new GotchiBotModel(gameObject.GetInstanceID(), "");
            model.OnUsernameUpdated += HandleUsernameUpdated;
            model.OnShouldSimulateDamageUpdated += HandleShouldSimulateDamageUpdated;
            model.OnHealthUpdated += HandleHealthUpdated;
        }

        void OnDestroy()
        {
            UserInterfaceManager.Instance.PlayersListUI.RemovePlayerEntry(model.Username);
            GotchiManager.Instance.RemoveBot(gameObject.GetInstanceID());
            model.OnUsernameUpdated -= HandleUsernameUpdated;
            model.OnShouldSimulateDamageUpdated -= HandleShouldSimulateDamageUpdated;
            model.OnHealthUpdated += HandleHealthUpdated;

        }
        #endregion

        #region Public Functions
        public void SetUsername(string username)
        {
            if (!model.Username.Equals(""))
            {
                UserInterfaceManager.Instance.PlayersListUI.RemovePlayerEntry(model.Username);
            }
            model.SetUsername(username);
        }

        public bool IsDead()
        {
            return model.Health <= 0;
        }
        #endregion

        #region Private Functions
        void HandleUsernameUpdated(string username)
        {
            if (!username.Equals(""))
            {
                UserInterfaceManager.Instance.PlayersListUI.AddPlayerEntry(gameObject.GetInstanceID(), username, false);
            }
        }

        void HandleShouldSimulateDamageUpdated(bool shouldSimulate)
        {
            if (shouldSimulate) {
                StartCoroutine(SimulateDamage());
            }
        }

        void HandleHealthUpdated(int health)
        {
            if (health <= 0)
            {
                EventBus.GotchiEvents.GotchiDied(gameObject.GetInstanceID());
            }
        }

        #endregion

        #region Coroutines
        private IEnumerator SimulateDamage()
        {
            while (model.ShouldSimulateDamage && model.Health > 0)
            {
                int damage = Math.Min(Random.Range(0, 15), model.Health);
                EventBus.GotchiEvents.GotchiDamaged(gameObject.GetInstanceID(), damage);
                model.SetHealth(model.Health - damage);
                yield return new WaitForSeconds(1f);
            }
        }
        #endregion
    }
}