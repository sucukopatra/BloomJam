using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Drives the right-hand SpriteRenderer that represents the current weapon.
    /// Sprite index follows ammo percentage: full mag = sprite[0], empty = last sprite.
    /// Switch/Reload restore sprite[0] and tint to WeaponData.TintColor.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class WeaponHandView : MonoBehaviour
    {
        [SerializeField, Tooltip("If true, the renderer is also tinted to WeaponData.TintColor on switch.")]
        private bool _applyTint = true;

        private SpriteRenderer _renderer;
        private WeaponData _data;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponSwitchedEvent>(OnSwitched);
            EventBus.Subscribe<WeaponFiredEvent>(OnFired);
            EventBus.Subscribe<WeaponReloadedEvent>(OnReloaded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponSwitchedEvent>(OnSwitched);
            EventBus.Unsubscribe<WeaponFiredEvent>(OnFired);
            EventBus.Unsubscribe<WeaponReloadedEvent>(OnReloaded);
        }

        private void OnSwitched(WeaponSwitchedEvent e)
        {
            _data = e.Weapon;
            ApplySpriteForAmmo(_data != null ? _data.MagazineSize : 0);
            if (_applyTint && _data != null)
                _renderer.color = _data.TintColor;
        }

        private void OnFired(WeaponFiredEvent e)
        {
            if (_data == null || e.Weapon != _data) return;
            ApplySpriteForAmmo(e.ShotsLeftInMag);
        }

        private void OnReloaded(WeaponReloadedEvent e)
        {
            if (_data == null || e.Weapon != _data) return;
            ApplySpriteForAmmo(_data.MagazineSize);
        }

        private void ApplySpriteForAmmo(int ammoLeft)
        {
            if (_data == null || _data.HandSpriteCycle == null || _data.HandSpriteCycle.Length == 0) return;

            int len = _data.HandSpriteCycle.Length;
            int magSize = Mathf.Max(1, _data.MagazineSize);
            float spent = 1f - (float)ammoLeft / magSize;          // 0 = full, 1 = empty
            int idx = Mathf.Clamp(Mathf.RoundToInt(spent * (len - 1)), 0, len - 1);
            _renderer.sprite = _data.HandSpriteCycle[idx];
        }
    }
}
