using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using Gotchi.Player.Presenter;

namespace Gotchi.Network
{
    public class NetworkListener : MonoBehaviour, INetworkRunnerCallbacks
    {
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("OnDisconnectedFromServer");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerJoined");

            if (runner.IsServer)
            {
                Debug.Log("We are server, spawning player");

                int botCount = GotchiManager.Instance.Bots.Count;
                if (botCount > 0) {
                    GotchiManager.Instance.RemoveBot();
                } else {
                    GotchiManager.Instance.SpawnBots(8 - runner.ActivePlayers.Count());
                }

                runner.Spawn(
                    GotchiManager.Instance.GotchiPrefab.GetComponent<GotchiPresenter>(),
                    GotchiManager.Instance.Spawn.position,
                    GotchiManager.Instance.Spawn.rotation,
                    player
                );
            }
            else
            {
                Debug.Log("We are NOT server");
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerLeft");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // if (!NetworkManager.Instance.IsReady) return;
            
            // input.Set(NetworkManager.Instance.NetworkTickData);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            // Debug.Log("OnInputMissing");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutdown");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("OnConnectFailed");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("OnUserSimulationMessage");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("OnSessionListUpdated");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("OnCustomAuthenticationResponse");
        }

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("OnHostMigration");

            await runner.Shutdown();

            SceneManager.LoadScene("GotchiTowerDefense-WithTerrain");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            Debug.Log("OnReliableDataReceived");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("OnSceneLoadStart");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("OnSceneLoadDone");
        }
    }
}