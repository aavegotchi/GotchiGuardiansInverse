using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Gotchi.Network
{
    public class NetworkGotchi : NetworkBehaviour, IPlayerLeft
    {
        [Networked(OnChanged = nameof(OnUsernameSet))]
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
                rpc_setUsername("");

                Debug.Log("Local player left, despawning");
                Runner.Despawn(Object);
            }
            else
            {
                Debug.Log("Remote player left");
            }
        }

        public static void OnUsernameSet(Changed<NetworkGotchi> changed)
        {
            changed.Behaviour.OnUsernameSet();
        }

        private void OnUsernameSet()
        {
            if (Username.ToString() == string.Empty)
            {
                Debug.Log($"Removing player {gameObject.name} from player list");
                UserInterfaceManager.Instance.PlayersListUI.RemovePlayerEntry(Username.ToString());
            }
            else
            {
                Debug.Log($"Adding player with {Username} for object {gameObject.name} to player list");
                UserInterfaceManager.Instance.PlayersListUI.AddPlayerEntry(Username.ToString());
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void rpc_setUsername(string username, RpcInfo info = default)
        {
            Username = username;
        }
    }
}