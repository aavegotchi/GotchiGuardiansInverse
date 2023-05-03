using UnityEngine;
using Fusion;

namespace Gotchi.Network
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 movementOffset;
        public Vector3 movementDestination;
    }
}