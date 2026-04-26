using UnityEngine;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Bridges FPSController state into Animator parameters so weapon-hand animations
    /// (idle, walk, run, crouch, jump, fall, land) can react to player movement.
    ///
    /// Author the Animator with these parameters:
    ///   float Speed       — horizontal velocity magnitude
    ///   float SpeedNorm   — Speed remapped 0..1 against MaxSpeed for clean blend trees
    ///   bool  Grounded    — true while standing on ground
    ///   bool  Sprinting   — true while sprint is held
    ///   bool  Crouching   — true while crouched
    ///   bool  Falling     — true while in PlayerState.Falling
    ///   trigger Jump      — fires once on the rising edge into PlayerState.Jumping
    ///   trigger Land      — fires once when transitioning from Falling/Jumping back to grounded
    ///
    /// Weapon-driven triggers (Switch/Reload/Fire/DryFire) live on WeaponAnimatorRelay
    /// and should be on a separate Animator layer with an empty avatar mask so they
    /// don't fight movement states.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class WeaponMotionAnimator : MonoBehaviour
    {
        [SerializeField, Tooltip("FPSController to read state from. Auto-resolved from parents on Awake if left empty.")]
        private FPSController _fps;

        [SerializeField, Tooltip("Smoothing time for SpeedNorm transitions between idle/walk/run.")]
        [Min(0f)] private float _speedNormSmoothTime = 0.1f;

        [SerializeField, Tooltip("How long the air/ground state must hold before Jump/Land triggers fire. Prevents flickering ground checks from re-firing triggers mid-jump.")]
        [Min(0f)] private float _airDebounceTime = 0.05f;

        private Animator _animator;

        private static readonly int HashSpeed     = Animator.StringToHash("Speed");
        private static readonly int HashSpeedNorm = Animator.StringToHash("SpeedNorm");
        private static readonly int HashGrounded  = Animator.StringToHash("Grounded");
        private static readonly int HashSprinting = Animator.StringToHash("Sprinting");
        private static readonly int HashFalling   = Animator.StringToHash("Falling");
        private static readonly int HashJump      = Animator.StringToHash("Jump");
        private static readonly int HashLand      = Animator.StringToHash("Land");

        private bool _stableAir;
        private float _airChangeTimer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_fps == null) _fps = GetComponentInParent<FPSController>();
            if (_fps == null)
                Debug.LogError($"[{nameof(WeaponMotionAnimator)}] No FPSController found in parents. Assign one in the inspector.");
        }

        private void OnEnable()
        {
            if (_fps != null) _stableAir = IsAirState(_fps.State);
            _airChangeTimer = 0f;
        }

        private static bool IsAirState(PlayerState s)
            => s == PlayerState.Falling || s == PlayerState.Jumping;

        private void Update()
        {
            if (_fps == null || _animator == null) return;

            Vector3 v = _fps.Velocity;
            v.y = 0f;
            float speed = v.magnitude;

            _animator.SetFloat(HashSpeed, speed);

            float speedNormTarget = speed < 0.1f ? 0f : (_fps.IsSprinting ? 1f : 0.5f);
            _animator.SetFloat(HashSpeedNorm, speedNormTarget, _speedNormSmoothTime, Time.deltaTime);
            _animator.SetBool(HashGrounded, _fps.IsGrounded);
            _animator.SetBool(HashSprinting, _fps.IsSprinting);
            _animator.SetBool(HashFalling, _fps.State == PlayerState.Falling);

            // Debounced air-state edge triggers — brief IsGrounded flickers from the
            // CheckSphere ground probe must NOT re-fire Jump/Land mid-jump.
            bool airNow = IsAirState(_fps.State);

            if (airNow == _stableAir)
            {
                _airChangeTimer = 0f;
            }
            else
            {
                _airChangeTimer += Time.deltaTime;
                if (_airChangeTimer >= _airDebounceTime)
                {
                    _stableAir = airNow;
                    _airChangeTimer = 0f;

                    if (_stableAir)
                    {
                        _animator.ResetTrigger(HashLand);
                        _animator.SetTrigger(HashJump);
                    }
                    else
                    {
                        _animator.ResetTrigger(HashJump);
                        _animator.SetTrigger(HashLand);
                    }
                }
            }
        }
    }
}
