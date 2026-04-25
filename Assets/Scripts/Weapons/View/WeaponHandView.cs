using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Drives the right-hand SpriteRenderer that represents the current weapon.
    /// Each shot advances through WeaponData.HandSpriteCycle (clamped). Reload restores index 0.
    /// Switch swaps to the new weapon's first sprite and tints to its color.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class WeaponHandView : MonoBehaviour
    {
        [SerializeField, Tooltip("If true, the renderer is also tinted to WeaponData.TintColor on switch.")]
        private bool _applyTint = true;

        private SpriteRenderer _renderer;
        private WeaponData _data;
        private int _shotIndex;

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
            _shotIndex = 0;
            ApplyCurrentSprite();
            if (_applyTint && _data != null)
                _renderer.color = _data.TintColor;
        }

        private void OnFired(WeaponFiredEvent e)
        {
            if (_data == null || e.Weapon != _data) return;
            _shotIndex++;
            ApplyCurrentSprite();
        }

        private void OnReloaded(WeaponReloadedEvent e)
        {
            if (_data == null || e.Weapon != _data) return;
            _shotIndex = 0;
            ApplyCurrentSprite();
        }

        private void ApplyCurrentSprite()
        {
            if (_data == null || _data.HandSpriteCycle == null || _data.HandSpriteCycle.Length == 0) return;
            int idx = Mathf.Clamp(_shotIndex, 0, _data.HandSpriteCycle.Length - 1);
            _renderer.sprite = _data.HandSpriteCycle[idx];
        }
    }
}
