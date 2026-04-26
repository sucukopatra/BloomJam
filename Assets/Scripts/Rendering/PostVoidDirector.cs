using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using BloomJam.Weapons;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Rendering
{
    /// <summary>
    /// Drives the PostVoid look every frame. Two channels:
    ///   1. Continuous (smoothed): player horizontal speed -> bloom intensity, CA, warp.
    ///   2. Impulse (decaying):    weapon-fire / damage events -> _PV_Pulse + lens distortion punch.
    ///
    /// Master toggle (`Effects Enabled`) flips the volume components' .active flag and
    /// the shader's gate uniform, so the look snaps from "PostVoid on" to "vanilla URP" instantly.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public sealed class PostVoidDirector : MonoBehaviour
    {
        [Header("Master toggle")]
        [Tooltip("If off, all PostVoid effects (volume overrides + custom shader pass) snap to neutral.")]
        [SerializeField] private bool _effectsEnabled = true;

        [Tooltip("Hotkey for runtime toggle. Set to None to disable.")]
        [SerializeField] private Key _toggleKey = Key.F1;

        [Header("Refs")]
        [Tooltip("Volume holding the post-process profile to drive (typically the global scene volume).")]
        [SerializeField] private Volume _volume;

        [Tooltip("Player FPS controller. Velocity is read each frame to derive the speed channel.")]
        [SerializeField] private FPSController _player;

        [Header("Speed channel")]
        [Tooltip("Player horizontal speed at which intensity reaches 1.")]
        [SerializeField] private float _maxSpeed = 12f;

        [Tooltip("Smoothing rate for the speed channel. Higher = snappier.")]
        [SerializeField] private float _intensitySmoothing = 6f;

        [Header("Pulse channel")]
        [Tooltip("How fast the impulse decays (1/sec). Higher = shorter punch.")]
        [SerializeField] private float _pulseDecay = 4f;

        [Tooltip("Pulse strength added on each WeaponFiredEvent.")]
        [SerializeField] private float _firePulse = 0.2f;

        [Tooltip("Subscribe to WeaponFiredEvent for muzzle-flash pulse.")]
        [SerializeField] private bool _pulseOnFire = true;

        [Header("Volume baselines")]
        [SerializeField] private float _baselineCA          = 0.05f;
        [SerializeField] private float _baselineBloom       = 0.4f;
        [SerializeField] private float _baselineLens        = -0.05f;
        [SerializeField] private float _baselineSaturation  = 5f;

        [Header("Volume max-add (at intensity=1, pulse=1)")]
        [SerializeField] private float _caFromSpeed         = 0.2f;
        [SerializeField] private float _caFromPulse         = 0.15f;
        [SerializeField] private float _bloomFromSpeed      = 0.8f;
        [SerializeField] private float _bloomFromPulse      = 1.0f;
        [SerializeField] private float _lensFromPulse       = -0.35f;
        [SerializeField] private float _saturationFromPulse = 25f;

        [Header("Pixelation")]
        [Tooltip("Edge length of one chunky pixel, in screen pixels. 1 = off, 4 = chunky, 8 = very chunky.")]
        [SerializeField, Range(1f, 16f)] private float _pixelSize = 4f;

        [Header("Shader (custom pass)")]
        [SerializeField] private float _shaderCABase        = 0.3f;
        [SerializeField] private float _shaderCAFromSpeed   = 1.5f;
        [SerializeField] private float _shaderCAFromPulse   = 4.0f;
        [SerializeField] private float _shaderWarpBase      = 0.001f;
        [SerializeField] private float _shaderWarpFromSpeed = 0.004f;
        [SerializeField] private float _shaderWarpFromPulse = 0.014f;

        // Cached overrides
        private ChromaticAberration _ca;
        private Bloom _bloom;
        private LensDistortion _lens;
        private ColorAdjustments _color;
        private Vignette _vignette;
        private FilmGrain _grain;

        // State
        private float _intensity;
        private float _pulse;
        private float _time;
        private bool _appliedEnabledState;

        // Shader IDs
        private static readonly int IntensityID = Shader.PropertyToID("_PV_Intensity");
        private static readonly int CAID        = Shader.PropertyToID("_PV_CA");
        private static readonly int WarpID      = Shader.PropertyToID("_PV_Warp");
        private static readonly int PulseID     = Shader.PropertyToID("_PV_Pulse");
        private static readonly int TimeID      = Shader.PropertyToID("_PV_Time");
        private static readonly int ShearID     = Shader.PropertyToID("_PV_Shear");
        private static readonly int PixelSizeID = Shader.PropertyToID("_PV_PixelSize");
        private static readonly int EnabledID   = Shader.PropertyToID("_PV_Enabled");

        /// <summary>Master toggle. Setter snaps both volume + shader state.</summary>
        public bool EffectsEnabled
        {
            get => _effectsEnabled;
            set
            {
                if (_effectsEnabled == value) return;
                _effectsEnabled = value;
                ApplyEnabledState(value);
            }
        }

        /// <summary>Flip the master toggle. Wire to a UI Button via OnClick.</summary>
        public void Toggle() => EffectsEnabled = !_effectsEnabled;

        private void Awake()
        {
            if (_volume == null) _volume = GetComponent<Volume>();
            CacheOverrides();
        }

        private void OnEnable()
        {
            if (_pulseOnFire)
                EventBus.Subscribe<WeaponFiredEvent>(OnWeaponFired);

            // Push the current state explicitly so toggling component on/off in inspector takes effect.
            ApplyEnabledState(_effectsEnabled);
        }

        private void OnDisable()
        {
            if (_pulseOnFire)
                EventBus.Unsubscribe<WeaponFiredEvent>(OnWeaponFired);

            // Leave the scene in the "off" state so disabling this component reverts the look.
            ApplyEnabledState(false);
        }

        private void CacheOverrides()
        {
            if (_volume == null || _volume.sharedProfile == null) return;
            _volume.sharedProfile.TryGet(out _ca);
            _volume.sharedProfile.TryGet(out _bloom);
            _volume.sharedProfile.TryGet(out _lens);
            _volume.sharedProfile.TryGet(out _color);
            _volume.sharedProfile.TryGet(out _vignette);
            _volume.sharedProfile.TryGet(out _grain);
        }

        private void OnWeaponFired(WeaponFiredEvent _) => HitPulse(_firePulse);

        /// <summary>Add an impulse to the pulse channel (clamps to 1).</summary>
        public void HitPulse(float strength = 1f)
        {
            if (!_effectsEnabled) return;
            _pulse = Mathf.Clamp01(Mathf.Max(_pulse, strength));
        }

        private void Update()
        {
            if (_toggleKey != Key.None && Keyboard.current != null
                && Keyboard.current[_toggleKey].wasPressedThisFrame)
            {
                Toggle();
            }

            if (!_effectsEnabled)
            {
                // Keep clearing transient channels so a re-enable starts cleanly.
                _intensity = 0f;
                _pulse = 0f;
                return;
            }

            float speed01 = ComputeSpeed01();
            _intensity = Mathf.Lerp(_intensity, speed01, Time.deltaTime * _intensitySmoothing);
            _pulse = Mathf.Max(0f, _pulse - Time.deltaTime * _pulseDecay);
            _time += Time.unscaledDeltaTime;

            ApplyToVolume();
            ApplyToShader();
        }

        private float ComputeSpeed01()
        {
            if (_player == null) return 0f;
            Vector3 v = _player.Velocity;
            v.y = 0f;
            return Mathf.Clamp01(v.magnitude / Mathf.Max(0.01f, _maxSpeed));
        }

        private void ApplyToVolume()
        {
            if (_ca != null)
                _ca.intensity.value = _baselineCA + _intensity * _caFromSpeed + _pulse * _caFromPulse;

            if (_bloom != null)
                _bloom.intensity.value = _baselineBloom + _intensity * _bloomFromSpeed + _pulse * _bloomFromPulse;

            if (_lens != null)
                _lens.intensity.value = _baselineLens + _pulse * _lensFromPulse;

            if (_color != null)
                _color.saturation.value = _baselineSaturation + _pulse * _saturationFromPulse;
        }

        private void ApplyToShader()
        {
            Shader.SetGlobalFloat(IntensityID, _intensity);
            Shader.SetGlobalFloat(CAID,        _shaderCABase   + _intensity * _shaderCAFromSpeed   + _pulse * _shaderCAFromPulse);
            Shader.SetGlobalFloat(WarpID,      _shaderWarpBase + _intensity * _shaderWarpFromSpeed + _pulse * _shaderWarpFromPulse);
            Shader.SetGlobalFloat(PulseID,     _pulse);
            Shader.SetGlobalFloat(TimeID,      _time);
            Shader.SetGlobalVector(ShearID,    Random.insideUnitCircle);
            Shader.SetGlobalFloat(PixelSizeID, _pixelSize);
        }

        /// <summary>
        /// Snap the look to enabled or disabled. Toggles the volume components' .active flag
        /// (which disables them across every Volume that references this profile), zeros the
        /// shader-driven channels, and flips the in-shader gate uniform.
        /// </summary>
        private void ApplyEnabledState(bool enabled)
        {
            if (_ca != null)       _ca.active       = enabled;
            if (_bloom != null)    _bloom.active    = enabled;
            if (_lens != null)     _lens.active     = enabled;
            if (_color != null)    _color.active    = enabled;
            if (_vignette != null) _vignette.active = enabled;
            if (_grain != null)    _grain.active    = enabled;

            Shader.SetGlobalFloat(EnabledID, enabled ? 1f : 0f);

            if (!enabled)
            {
                // Push neutral values so the next frame doesn't briefly flash with stale data.
                Shader.SetGlobalFloat(IntensityID, 0f);
                Shader.SetGlobalFloat(CAID,        0f);
                Shader.SetGlobalFloat(WarpID,      0f);
                Shader.SetGlobalFloat(PulseID,     0f);
                Shader.SetGlobalFloat(PixelSizeID, 1f);
            }

            _appliedEnabledState = enabled;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_volume == null) _volume = GetComponent<Volume>();

            // Mirror inspector toggling of _effectsEnabled in play mode.
            if (Application.isPlaying && _appliedEnabledState != _effectsEnabled)
            {
                CacheOverrides();
                ApplyEnabledState(_effectsEnabled);
            }
        }
#endif
    }
}
