using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Enemies
{
    public struct EnemyDiedEvent : IEvent
    {
        public EnemyType Type;
        public int PairingId;
        public Vector3 Position;
    }
}
