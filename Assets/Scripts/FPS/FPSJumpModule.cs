using UnityEngine;

public class FPSJumpModule : FPSModule
{
    [SerializeField] private float jumpHeight     = 1.2f;
    [SerializeField] private float coyoteTime     = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private int   maxJumps       = 2;

    public override int ExecutionOrder => 3;

    private float _gravity;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private int   _jumpsLeft;
    private float _jumpCooldown;
    private bool  _prevGrounded;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        _gravity   = Physics.gravity.y;
        _jumpsLeft = 0;
    }

    public override void Tick()
    {
        UpdateGroundState();
        UpdateJumpBuffer();
        ApplyJump();
    }

    private void UpdateGroundState()
    {
        _jumpCooldown -= Time.deltaTime;

        // Zıpladıktan sonra 0.2s boyunca zemin tespitini yok say.
        // Bu sayede IsGrounded'ın "zıplamadan hemen sonra hâlâ true döndürme"
        // sorunu jump sayacını bozmaz.
        bool grounded = Controller.IsGrounded && _jumpCooldown <= 0f;

        if (grounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        // Yere ilk değdiğimiz anda (hava → yer geçişi) zıplama hakkını yenile
        if (grounded && !_prevGrounded)
            _jumpsLeft = maxJumps;

        _prevGrounded = grounded;
    }

    private void UpdateJumpBuffer()
    {
        if (Controller.Input.JumpPressed)
        {
            _jumpBufferCounter = jumpBufferTime;
            Controller.Input.ConsumeJump();
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void ApplyJump()
    {
        // Coyote penceresi: yerde ya da az önce yerde → ilk zıplama
        // Extra zıplama: coyote bitti ama hak kaldı → double jump
        bool canJump   = _coyoteTimeCounter > 0f || (_jumpsLeft > 0 && _coyoteTimeCounter <= 0f);
        bool wantsJump = _jumpBufferCounter > 0f;

        if (canJump && wantsJump)
        {
            Controller.Velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
            _coyoteTimeCounter = 0f;
            _jumpBufferCounter = 0f;
            _jumpsLeft--;
            _jumpCooldown = 0.2f; // zıpladıktan sonra 0.2s zemin tespiti gecikmesi
        }
    }
}