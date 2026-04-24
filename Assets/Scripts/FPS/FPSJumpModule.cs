using System.Collections.Generic;
using UnityEngine;

public class FPSJumpModule : FPSModule
{
    [SerializeField] private float jumpHeight    = 1.2f;
    [SerializeField] private float coyoteTime    = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

public override int ExecutionOrder => 3; // Movement'tan sonra, Gravity'den önce

    private float _gravity;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        _gravity = Physics.gravity.y;
    }

    public override void Tick()
    {
        UpdateCoyoteTime();
        UpdateJumpBuffer();
        ApplyJump();
    }

    private void UpdateCoyoteTime()
    {
        if (Controller.IsGrounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;
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
        bool canJump   = _coyoteTimeCounter > 0f;
        bool wantsJump = _jumpBufferCounter > 0f;

        if (canJump && wantsJump)
        {
           
            Controller.Velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);

            _coyoteTimeCounter = 0f;
            _jumpBufferCounter = 0f;
        }
    }
}