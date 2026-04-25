using UnityEngine;

namespace BloomJam.Enemies
{
    public class FlyingMovement : MonoBehaviour, IEnemyMovement
    {
        [SerializeField] private float speed        = 4f;
        [SerializeField] private float hoverHeight  = 2.5f;
        [SerializeField] private float bobAmplitude = 0.2f;
        [SerializeField] private float bobFrequency = 2f;

        private Vector3 _velocity;
        public Vector3 Velocity => _velocity;

        public void MoveTowards(Vector3 worldTarget)
        {
            var aimPoint = worldTarget + Vector3.up * hoverHeight;
            var dir      = (aimPoint - transform.position).normalized;
            _velocity    = dir * speed;
        }

        public void Stop() => _velocity = Vector3.zero;

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            var bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude * Time.deltaTime;
            transform.position += Vector3.up * bob;
        }
    }
}