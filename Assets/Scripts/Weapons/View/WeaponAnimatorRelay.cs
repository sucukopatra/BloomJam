using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Forwards weapon events to a single Animator as triggers. Trigger names live on WeaponData,
    /// so animator state machines can be built later without touching gameplay code.
    /// Empty trigger strings are skipped.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class WeaponAnimatorRelay : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponSwitchStartedEvent>(OnSwitch);
            EventBus.Subscribe<WeaponReloadStartedEvent>(OnReload);
            EventBus.Subscribe<WeaponFiredEvent>(OnFired);
            EventBus.Subscribe<WeaponDryFireEvent>(OnDryFire);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponSwitchStartedEvent>(OnSwitch);
            EventBus.Unsubscribe<WeaponReloadStartedEvent>(OnReload);
            EventBus.Unsubscribe<WeaponFiredEvent>(OnFired);
            EventBus.Unsubscribe<WeaponDryFireEvent>(OnDryFire);
        }

        private void OnSwitch(WeaponSwitchStartedEvent e) => Trigger(e.Next   != null ? e.Next.SwitchAnimTrigger   : null);
        private void OnReload(WeaponReloadStartedEvent e) => Trigger(e.Weapon != null ? e.Weapon.ReloadAnimTrigger : null);
        private void OnFired(WeaponFiredEvent e)          => Trigger(e.Weapon != null ? e.Weapon.FireAnimTrigger   : null);
        private void OnDryFire(WeaponDryFireEvent e)      => Trigger(e.Weapon != null ? e.Weapon.DryFireAnimTrigger : null);

        private void Trigger(string name)
        {
            if (_animator == null || string.IsNullOrEmpty(name)) return;
            _animator.SetTrigger(name);
        }
    }
}
