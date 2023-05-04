using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

namespace Gotchi.Network
{
    public class NetworkManager : NetworkBehaviour
    {
        #region Public Variables
        public static NetworkManager Instance = null;

        public Player_Gotchi LocalPlayerGotchi { get; set; }
        public NetworkGotchiInput LocalPlayerInput { get; set; }
        public Player_Gotchi RemotePlayerGotchi { get; set; }
        public NetworkGotchiInput RemotePlayerInput { get; set; }

        public NetworkRunner NetworkRunner
        {
            get { return networkRunner; }
        }
        public bool IsReady {
            get { return LocalPlayerGotchi != null; }
        }

        public NetworkTickData NetworkTickData = new NetworkTickData();
        #endregion

        #region Fields
        [SerializeField] private NetworkRunner networkRunnerPrefab = null;
        [SerializeField] private int maxPlayerCount = 8;
        #endregion

        #region Private Variables
        private NetworkRunner networkRunner = null;
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
            if (networkRunner == null)
            {
                networkRunner = Instantiate(networkRunnerPrefab);
                networkRunner.name = "network_listener";
                networkRunner.ProvideInput = true;
            }

            var sceneManager = networkRunner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (sceneManager == null)
            {
                sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            Debug.Log("Server started");

            networkRunner.StartGame(new StartGameArgs
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
        #endregion
    }
}
