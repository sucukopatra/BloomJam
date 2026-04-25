using UnityEngine;

namespace BloomJam.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class WalkingMovement : MonoBehaviour, IEnemyMovement
    {
        [SerializeField] private float speed = 3f;

        private Rigidbody _rb;
        private Vector3   _desiredVelocity;

        public Vector3 Velocity => _rb.linearVelocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }

        public void MoveTowards(Vector3 worldTarget)
        {
            var dir = worldTarget - transform.position;
            dir.y = 0f;
            _desiredVelocity = dir.normalized * speed;
        }

        public void Stop() => _desiredVelocity = Vector3.zero;

        private void FixedUpdate()
        {
            var v = _rb.linearVelocity;
            v.x = _desiredVelocity.x;
            v.z = _desiredVelocity.z;
            _rb.linearVelocity = v;
        }
    }
}