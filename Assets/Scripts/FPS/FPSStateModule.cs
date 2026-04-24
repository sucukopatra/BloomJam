using UnityEngine;

public class FPSStateModule : FPSModule
{
    public override int ExecutionOrder => 5; // Gravity(4) sonrası, Camera(6) öncesi

    public override void Tick()
    {
        Controller.PreviousState = Controller.State;
        Controller.State = DetermineState();
    }

    private PlayerState DetermineState()
    {
        if (!Controller.IsGrounded && Controller.Velocity.y > 0f)  return PlayerState.Jumping;
        if (!Controller.IsGrounded && Controller.Velocity.y <= 0f) return PlayerState.Falling;
        if (Controller.IsCrouching)                                return PlayerState.Crouching;
        if (Controller.IsSprinting)                                return PlayerState.Sprinting;

        // Vector2 allocation yok — sqrMagnitude eşdeğeri
        bool isMoving = (Controller.Velocity.x * Controller.Velocity.x
                       + Controller.Velocity.z * Controller.Velocity.z) > 0.01f;

        return isMoving ? PlayerState.Walking : PlayerState.Idle;
    }
}