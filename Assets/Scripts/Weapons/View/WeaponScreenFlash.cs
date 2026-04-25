using UnityEngine;
using UnityEngine.UI;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Briefly flashes a fullscreen UI Image to the weapon's screen-flash color/intensity/duration.
    /// Wire to a Canvas Image set to stretch fullscreen, raycastTarget = false, alpha = 0.
    /// </summary>
    public sealed class WeaponScreenFlash : MonoBehaviour
    {
        [SerializeField, Tooltip("Fullscreen UI image. Alpha will be driven on each shot.")]
        private Image _flashImage;

        private float _elapsed;
        private float _duration;
        private Color _baseColor;
        private float _peakAlpha;

        private void OnEnable()  => EventBus.Subscribe<WeaponFiredEvent>(OnFired);
        private void OnDisable() => EventBus.Unsubscribe<WeaponFiredEvent>(OnFired);

        private void Reset()
        {
            _flashImage = GetComponentInChildren<Image>();
        }

        private void OnFired(WeaponFiredEvent e)
        {
            if (_flashImage == null || e.Weapon == null) return;
            _baseColor = e.Weapon.ScreenFlashColor;
            _peakAlpha = Mathf.Clamp01(e.Weapon.ScreenFlashIntensity);
            _duration  = Mathf.Max(0.0001f, e.Weapon.ScreenFlashDuration);
            _elapsed   = 0f;
        }

        private void Update()
        {
            if (_flashImage == null) return;

            if (_elapsed >= _duration)
            {
                if (_flashImage.color.a > 0f)
                {
                    var c = _baseColor; c.a = 0f;
                    _flashImage.color = c;
                }
                return;
            }

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            float a = Mathf.Lerp(_peakAlpha, 0f, t);
            var col = _baseColor; col.a = a;
            _flashImage.color = col;
        }
    }
}
