using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Linq;
using Gotchi.Player.Presenter;

namespace Gotchi.Network
{
    public class NetworkManager : NetworkBehaviour
    {
        #region Public Variables
        public static NetworkManager Instance = null;

        public GotchiPresenter LocalPlayerGotchi { get; set; }

        public bool IsReady 
        {
            get { return LocalPlayerGotchi != null; }
        }
        #endregion

        #region
        private NetworkRunner networkRunnerInstance = null;
        #endregion

        #region Fields
        [SerializeField] private NetworkRunner networkRunnerPrefab = null;
        [SerializeField] private int maxPlayerCount = 8;
        #endregion

        #region Unity Functions
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Public Functions
        public void InitializeNetworkRunner(string lobbyId)
        {
            if (networkRunnerInstance == null) {
                networkRunnerInstance = FindObjectOfType<NetworkRunner>();

                if (networkRunnerInstance == null) {
                    networkRunnerInstance = Instantiate(networkRunnerPrefab);
                }
            }
            
            networkRunnerInstance.name = "network_listener";
            networkRunnerInstance.ProvideInput = true;

            var sceneManager = getSceneManager(networkRunnerInstance);

            Debug.Log("Server started");

            networkRunnerInstance.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                Address = NetAddress.Any(),
                Scene = SceneManager.GetActiveScene().buildIndex,
                SessionName = lobbyId,
                Initialized = null,
                SceneManager = sceneManager,
                PlayerCount = maxPlayerCount
            });
        }

        public void LeaveMultiplayerRoom()
        {
            networkRunnerInstance.Shutdown();
        }
        #endregion
        
        #region Private Functions
        private INetworkSceneManager getSceneManager(NetworkRunner runner)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (sceneManager == null)
            {
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            return sceneManager;
        }
        #endregion
    }
}
