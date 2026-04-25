using UnityEngine;

namespace BloomJam.Enemies
{
    public class EnemyBillboard : MonoBehaviour
    {
        [SerializeField] private bool lockYAxis = true;
        private Transform _cam;

        private void Start()
        {
            _cam = Camera.main != null ? Camera.main.transform : null;
        }

        private void LateUpdate()
        {
            if (_cam == null) return;

            if (lockYAxis)
            {
                var dir = transform.position - _cam.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }
            else
            {
                transform.rotation = _cam.rotation;
            }
        }
    }
}