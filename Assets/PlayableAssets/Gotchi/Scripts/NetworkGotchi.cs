using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Gotchi.Network
{
    public class NetworkGotchi : NetworkBehaviour, IPlayerLeft
    {
        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManager.Instance.LocalPlayerGotchi = GetComponent<Player_Gotchi>();
                NetworkManager.Instance.LocalPlayerInput = GetComponent<NetworkGotchiInput>();
                Debug.Log("Spawned local player");
            }
            else
            {
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
        }
    }
}