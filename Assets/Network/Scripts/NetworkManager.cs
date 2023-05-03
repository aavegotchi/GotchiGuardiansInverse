using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Gotchi.Network
{
    public class NetworkManager : MonoBehaviour
    {
        #region Public Variables
        public static NetworkManager Instance = null;

        public Player_Gotchi LocalPlayerGotchi { get; set; } = null;
        public NetworkGotchiInput LocalPlayerInput { get; set; } = null;

        public NetworkInputData NetworkTickData = new NetworkInputData();
        #endregion

        #region Fields
        [SerializeField] private NetworkRunner networkRunnerPrefab = null;
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

        void Start()
        {
            NetworkRunner networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "network_listener";
            networkRunner.ProvideInput = true;

            initializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

            Debug.Log("Server started");
        }
        #endregion

        #region Private Functions
        private Task initializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (sceneManager == null)
            {
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            return runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "test_room",
                Initialized = initialized,
                SceneManager = sceneManager
            });
        }
        #endregion
    }
}
