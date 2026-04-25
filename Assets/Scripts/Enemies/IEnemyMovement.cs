using UnityEngine;

namespace BloomJam.Enemies
{
    public interface IEnemyMovement
    {
        void MoveTowards(Vector3 worldTarget);
        void Stop();
        Vector3 Velocity { get; }
    }
}