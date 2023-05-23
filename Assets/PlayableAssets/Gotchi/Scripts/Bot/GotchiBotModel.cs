using System;
using GameMaster;
using PhaseManager;

namespace Gotchi.Bot.Model
{
    public class GotchiBotModel
    {
        #region Events
        public event Action<int> OnGotchiIdUpdated = delegate { };
        public event Action<int> OnHealthUpdated = delegate { };
        public event Action<string> OnUsernameUpdated = delegate { };
        public event Action<bool> OnShouldSimulateDamageUpdated = delegate { };
        #endregion

        #region Public Variables
        public int GotchiId { get {return gotchiId; } }
        public int Health { get { return health; } }
        public string Username { get { return username; } }
        public bool ShouldSimulateDamage { get { return shouldSimulateDamage; } }
        #endregion

        #region Private Variables
        private int gotchiId;
        private int health = 0;
        private string username;
        private bool shouldSimulateDamage = false;
        #endregion

        #region  Public Methods
        public GotchiBotModel(int gotchiId, string username, int health = 200) {
            this.gotchiId = gotchiId;
            this.health = health;
            this.username = username;
            GameMasterEvents.PhaseEvents.PhaseChanged += HandlePhaseUpdated;
        }

        public void SetGotchiId(int gotchiId)
        {
            this.gotchiId = gotchiId;

            OnGotchiIdUpdated(gotchiId);
        }

        public void SetHealth(int health)
        {
            this.health = health;

            OnHealthUpdated(health);
        }

        public void SetUsername(string username)
        {
            this.username = username;

            OnUsernameUpdated(username);
        }

        public void SetShouldSimulateDamage(bool shouldSimulateDamage)
        {
            this.shouldSimulateDamage = shouldSimulateDamage;

            OnShouldSimulateDamageUpdated(shouldSimulateDamage);
        }

        #endregion

        #region Private Methods
        void HandlePhaseUpdated(Phase phase) {
            if (phase == Phase.Survival) {
                SetShouldSimulateDamage(true);
            } else {
                SetShouldSimulateDamage(false);
            }
        }
        #endregion
    }
}