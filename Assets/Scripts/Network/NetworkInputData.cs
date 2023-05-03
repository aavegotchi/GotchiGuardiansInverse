using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 movementOffset;
    public Vector3 movementDestination;
    public NetworkBool spinAttackTriggered;
}