using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

namespace Gotchi.Network
{
    public class NetworkGotchi : NetworkBehaviour, IPlayerLeft
    {
        [Networked(OnChanged = nameof(OnSetUsername))] 
        public NetworkString<_16> Username { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManager.Instance.LocalPlayerGotchi = GetComponent<Player_Gotchi>();
                NetworkManager.Instance.LocalPlayerInput = GetComponent<NetworkGotchiInput>();
                Debug.Log("Spawned local player");
                rpc_setUsername(PlayerPrefs.GetString("username"));
            }
            else
            {
                NetworkManager.Instance.RemotePlayerGotchi = GetComponent<Player_Gotchi>();
                NetworkManager.Instance.RemotePlayerInput = GetComponent<NetworkGotchiInput>();
                Debug.Log("Spawned remote player");
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
            {
                Debug.Log("Local player left, despawning");
                Runner.Despawn(Object);
            }
            else
            {
                Debug.Log("Remote player left");
            }
            UserInterfaceManager.Instance.PlayersListUI.RemovePlayerEntry(Username.ToString());
            SceneManager.LoadScene("GotchiTowerDefense");
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void rpc_setUsername(string username, RpcInfo info = default)
        {
            Username = username;
        }

        public static void OnSetUsername(Changed<NetworkGotchi> changed)
        {
            changed.Behaviour.onSetUsername();
        }

        private void onSetUsername()
        {
            if (Object.HasInputAuthority)
            {
                UserInterfaceManager.Instance.PlayersListUI.AddPlayerEntry(Username.ToString(), true);
            }
            else
            {
                UserInterfaceManager.Instance.PlayersListUI.AddPlayerEntry(Username.ToString());
            }
        }
    }
}