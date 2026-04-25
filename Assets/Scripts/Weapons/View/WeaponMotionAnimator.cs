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

        [SerializeField, Tooltip("Speed used to normalize SpeedNorm (typically your sprint speed).")]
        [Min(0.01f)] private float _maxSpeed = 6f;

        private Animator _animator;

        private static readonly int HashSpeed     = Animator.StringToHash("Speed");
        private static readonly int HashSpeedNorm = Animator.StringToHash("SpeedNorm");
        private static readonly int HashGrounded  = Animator.StringToHash("Grounded");
        private static readonly int HashSprinting = Animator.StringToHash("Sprinting");
        private static readonly int HashCrouching = Animator.StringToHash("Crouching");
        private static readonly int HashFalling   = Animator.StringToHash("Falling");
        private static readonly int HashJump      = Animator.StringToHash("Jump");
        private static readonly int HashLand      = Animator.StringToHash("Land");

        private PlayerState _prevState;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_fps == null) _fps = GetComponentInParent<FPSController>();
            if (_fps == null)
                Debug.LogError($"[{nameof(WeaponMotionAnimator)}] No FPSController found in parents. Assign one in the inspector.");
        }

        private void OnEnable()
        {
            if (_fps != null) _prevState = _fps.State;
        }

        private void Update()
        {
            if (_fps == null || _animator == null) return;

            Vector3 v = _fps.Velocity;
            v.y = 0f;
            float speed = v.magnitude;

            _animator.SetFloat(HashSpeed, speed);
            _animator.SetFloat(HashSpeedNorm, Mathf.Clamp01(speed / _maxSpeed));
            _animator.SetBool(HashGrounded, _fps.IsGrounded);
            _animator.SetBool(HashSprinting, _fps.IsSprinting);
            _animator.SetBool(HashCrouching, _fps.IsCrouching);
            _animator.SetBool(HashFalling, _fps.State == PlayerState.Falling);

            // Edge triggers
            PlayerState now = _fps.State;
            bool wasAir   = _prevState == PlayerState.Falling || _prevState == PlayerState.Jumping;
            bool isAir    = now        == PlayerState.Falling || now        == PlayerState.Jumping;

            if (!wasAir && now == PlayerState.Jumping)
                _animator.SetTrigger(HashJump);

            if (wasAir && !isAir)
                _animator.SetTrigger(HashLand);

            _prevState = now;
        }
    }
}
