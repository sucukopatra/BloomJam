using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Plays one-shot audio for fire / reload / switch from the active WeaponData.
    /// All clips are TODO assets — missing clips are silently skipped.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class WeaponAudioPlayer : MonoBehaviour
    {
        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponFiredEvent>(OnFired);
            EventBus.Subscribe<WeaponReloadStartedEvent>(OnReloadStart);
            EventBus.Subscribe<WeaponSwitchStartedEvent>(OnSwitchStart);
            EventBus.Subscribe<WeaponDryFireEvent>(OnDryFire);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponFiredEvent>(OnFired);
            EventBus.Unsubscribe<WeaponReloadStartedEvent>(OnReloadStart);
            EventBus.Unsubscribe<WeaponSwitchStartedEvent>(OnSwitchStart);
            EventBus.Unsubscribe<WeaponDryFireEvent>(OnDryFire);
        }

        private void OnFired(WeaponFiredEvent e)        => Play(e.Weapon != null ? e.Weapon.FireClip   : null);
        private void OnReloadStart(WeaponReloadStartedEvent e) => Play(e.Weapon != null ? e.Weapon.ReloadClip : null);
        private void OnSwitchStart(WeaponSwitchStartedEvent e) => Play(e.Next   != null ? e.Next.SwitchClip   : null);
        private void OnDryFire(WeaponDryFireEvent e)    => Play(e.Weapon != null ? e.Weapon.DryFireClip : null);

        private void Play(AudioClip clip)
        {
            if (clip == null || _source == null) return;
            _source.PlayOneShot(clip);
        }
    }
}
