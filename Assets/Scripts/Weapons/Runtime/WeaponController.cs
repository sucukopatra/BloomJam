using System.Collections;
using UnityEngine;
using YigitcanCaliskan;
using YigitcanCaliskan.EventBus;
using YigitcanCaliskan.ServiceLocator;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Owns the active weapon, routes input from IInputService, drives the per-weapon strategy,
    /// publishes weapon events through EventBus, and exposes a read-only IWeaponService.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public sealed class WeaponController : MonoBehaviour, IWeaponService, IBootstrapService
    {
        [Header("Slots (1, 2, 3)")]
        [Tooltip("Three weapon definitions. Index 0 = slot 1, index 1 = slot 2, index 2 = slot 3.")]
        [SerializeField] private WeaponData[] _slots = new WeaponData[3];

        [Tooltip("Slot to equip on Start (1-based: 1, 2, or 3).")]
        [SerializeField, Range(1, 3)] private int _startingSlot = 1;

        [Header("Scene refs")]
        [Tooltip("Transform where shots originate (muzzle / fingertip). If null, falls back to AimSource then transform.")]
        [SerializeField] private Transform _fireOrigin;

        [Tooltip("Transform whose forward defines the aim direction. Usually the FPS camera.")]
        [SerializeField] private Transform _aimSource;

        [Header("Input source")]
        [Tooltip("Optional explicit input service. If null, resolves IInputService from ServiceLocator on enable.")]
        [SerializeField] private MonoBehaviour _inputServiceOverride;

        private IInputService _input;
        private IWeaponBehaviour _activeBehaviour;
        private WeaponRuntimeContext _activeCtx;
        private int _currentSlot = -1;
        private bool _isSwitching;
        private bool _isReloading;
        private Coroutine _switchRoutine;
        private Coroutine _reloadRoutine;

        public WeaponData CurrentWeapon => _activeCtx?.Data;
        public int CurrentSlot => _currentSlot + 1;
        public int CurrentAmmo => _activeCtx?.CurrentAmmo ?? 0;
        public int MaxAmmo => _activeCtx?.Data != null ? _activeCtx.Data.MagazineSize : 0;
        public bool IsBusy => _isSwitching || _isReloading;

        public void Register()
        {
            ServiceLocator.Replace<IWeaponService>(this);
        }

        private void OnEnable()
        {
            ResolveInput();
            if (_input == null)
            {
                Debug.LogError($"[{nameof(WeaponController)}] No IInputService available. Weapons will not respond to input.");
                return;
            }

            _input.OnAttack  += HandleAttackPressed;
            _input.OnReload  += HandleReloadPressed;
            _input.OnSlot1   += HandleSlot1;
            _input.OnSlot2   += HandleSlot2;
            _input.OnSlot3   += HandleSlot3;
        }

        private void OnDisable()
        {
            if (_input == null) return;
            _input.OnAttack  -= HandleAttackPressed;
            _input.OnReload  -= HandleReloadPressed;
            _input.OnSlot1   -= HandleSlot1;
            _input.OnSlot2   -= HandleSlot2;
            _input.OnSlot3   -= HandleSlot3;
        }

        private void Start()
        {
            int idx = Mathf.Clamp(_startingSlot - 1, 0, _slots.Length - 1);
            EquipSlot(idx, instant: true);
        }

        private void Update()
        {
            if (_activeBehaviour == null || IsBusy) return;

            // Edge-detect attack release for full-auto strategies.
            // OnAttack covers press; held/release we read from the polling property.
            if (_input != null && !_input.AttackHeld && _wasAttackHeldLastFrame)
                _activeBehaviour.OnAttackReleased();

            _wasAttackHeldLastFrame = _input != null && _input.AttackHeld;

            _activeBehaviour.Tick(Time.deltaTime);
        }

        private bool _wasAttackHeldLastFrame;

        private void ResolveInput()
        {
            if (_inputServiceOverride is IInputService overridden)
            {
                _input = overridden;
                return;
            }

            if (ServiceLocator.TryGet<IInputService>(out var svc))
                _input = svc;
        }

        // ---- Input handlers ---------------------------------------------------------

        private void HandleAttackPressed()
        {
            if (_activeBehaviour == null || IsBusy) return;
            _activeBehaviour.OnAttackPressed();
        }

        private void HandleReloadPressed()
        {
            RequestReload();
        }

        private void HandleSlot1() => RequestSwitch(0);
        private void HandleSlot2() => RequestSwitch(1);
        private void HandleSlot3() => RequestSwitch(2);

        // ---- Public API -------------------------------------------------------------

        public void RequestSwitch(int slotIndex)
        {
            if (IsBusy) return;
            if (slotIndex == _currentSlot) return;
            if (slotIndex < 0 || slotIndex >= _slots.Length) return;
            if (_slots[slotIndex] == null)
            {
                Debug.LogWarning($"[{nameof(WeaponController)}] Slot {slotIndex + 1} is empty.");
                return;
            }
            EquipSlot(slotIndex, instant: false);
        }

        public void RequestReload()
        {
            if (IsBusy || _activeCtx == null || _activeCtx.Data == null) return;
            if (_activeCtx.CurrentAmmo >= _activeCtx.Data.MagazineSize) return;
            if (_reloadRoutine != null) StopCoroutine(_reloadRoutine);
            _reloadRoutine = StartCoroutine(ReloadRoutine());
        }

        // ---- Switch & reload routines ----------------------------------------------

        private void EquipSlot(int slotIndex, bool instant)
        {
            WeaponData next = _slots[slotIndex];
            if (next == null)
            {
                Debug.LogError($"[{nameof(WeaponController)}] Cannot equip empty slot {slotIndex + 1}.");
                return;
            }

            if (_switchRoutine != null) StopCoroutine(_switchRoutine);
            _switchRoutine = StartCoroutine(SwitchRoutine(slotIndex, next, instant));
        }

        private IEnumerator SwitchRoutine(int slotIndex, WeaponData next, bool instant)
        {
            _isSwitching = true;
            float duration = instant ? 0f : Mathf.Max(0f, next.SwitchTime);

            EventBus.Publish(new WeaponSwitchStartedEvent(slotIndex + 1, next, duration));

            // Holster current
            _activeBehaviour?.OnHolster();

            if (duration > 0f)
                yield return new WaitForSeconds(duration);

            _currentSlot = slotIndex;
            _activeCtx = new WeaponRuntimeContext(this, next, _fireOrigin, _aimSource);
            _activeBehaviour = WeaponBehaviourFactory.Create(next.BehaviourKind);
            _activeBehaviour.Initialize(_activeCtx);
            _activeBehaviour.OnEquip();

            _isSwitching = false;
            _switchRoutine = null;

            EventBus.Publish(new WeaponSwitchedEvent(slotIndex + 1, next));
            EventBus.Publish(new AmmoChangedEvent(next, _activeCtx.CurrentAmmo, next.MagazineSize));
        }

        private IEnumerator ReloadRoutine()
        {
            _isReloading = true;
            WeaponData data = _activeCtx.Data;
            float duration = Mathf.Max(0f, data.ReloadTime);

            EventBus.Publish(new WeaponReloadStartedEvent(data, duration));

            if (duration > 0f)
                yield return new WaitForSeconds(duration);

            _activeCtx.CurrentAmmo = data.MagazineSize;
            _isReloading = false;
            _reloadRoutine = null;

            EventBus.Publish(new WeaponReloadedEvent(data));
            EventBus.Publish(new AmmoChangedEvent(data, _activeCtx.CurrentAmmo, data.MagazineSize));
        }
    }
}
