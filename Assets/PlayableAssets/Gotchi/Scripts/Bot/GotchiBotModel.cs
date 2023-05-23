using System;

namespace Gotchi.Bot.Model
{
    public class GotchiBotModel
    {
        #region Events
        public event Action<int> OnGotchiIdUpdated = delegate { };
        public event Action<int> OnHealthUpdated = delegate { };
        public event Action<string> OnUsernameUpdated = delegate { };
        #endregion

        #region Public Variables
        public int GotchiId { get {return gotchiId; } }
        public int Health { get { return health; } }
        public string Username { get { return username; } }
        #endregion

        #region Private Variables
        private int gotchiId;
        private int health = 0;
        private string username;
        #endregion

        #region  Public Methods
        public GotchiBotModel(int gotchiId, string username, int health = 100) {
            this.gotchiId = gotchiId;
            this.health = health;
            this.username = username;
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

        #endregion
    }
}