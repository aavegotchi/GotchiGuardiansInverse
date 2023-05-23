using System;
using System.Collections.Generic;
using System.Linq;
using Gotchi.Events;
using UnityEngine;
using PhaseManager;
using PhaseManager.Presenter;
using Gotchi.Bot.Model;

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
            model.OnUsernameUpdated += OnUsernameUpdated;
        }

        void OnDestroy()
        {
            UserInterfaceManager.Instance.PlayersListUI.RemovePlayerEntry(model.Username);
            model.OnUsernameUpdated -= OnUsernameUpdated;
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
        #endregion

        #region Private Functions
        void OnUsernameUpdated(string username)
        {
            if (!username.Equals(""))
            {
                UserInterfaceManager.Instance.PlayersListUI.AddPlayerEntry(gameObject.GetInstanceID(), username, false);
            }
        }
        #endregion
    }
}