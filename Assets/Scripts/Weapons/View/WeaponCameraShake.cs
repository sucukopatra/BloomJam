using Unity.Cinemachine;
using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Generates a Cinemachine impulse on each shot so it composes with the existing
    /// FPSCameraShakeModule (landing shake) without conflict.
    /// </summary>
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public sealed class WeaponCameraShake : MonoBehaviour
    {
        private CinemachineImpulseSource _impulse;

        private void Awake()
        {
            _impulse = GetComponent<CinemachineImpulseSource>();
        }

        private void OnEnable()  => EventBus.Subscribe<WeaponFiredEvent>(OnFired);
        private void OnDisable() => EventBus.Unsubscribe<WeaponFiredEvent>(OnFired);

        private void OnFired(WeaponFiredEvent e)
        {
            if (_impulse == null || e.Weapon == null) return;
            float force = Mathf.Max(0f, e.Weapon.ShakeForce);
            if (force <= 0f) return;
            _impulse.GenerateImpulse(force);
        }
    }
}
