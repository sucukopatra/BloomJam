using UnityEngine;
using Unity.Cinemachine;

public class FPSCameraShakeModule : FPSModule
{
    public override int ExecutionOrder => 9;

    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Landing Shake")]
    [SerializeField] private float softLandingForce              = 0.5f;
    [SerializeField] private float hardLandingForce              = 1.5f;
    [SerializeField] private float hardLandingVelocityThreshold  = 6f;

    private float _peakFallVelocity;

    public override void Tick()
    {
        // Düşerken en yüksek hızı takip et
        if (!Controller.IsGrounded && Controller.Velocity.y < _peakFallVelocity)
            _peakFallVelocity = Controller.Velocity.y;

        bool wasInAir   = Controller.PreviousState == PlayerState.Falling
                       || Controller.PreviousState == PlayerState.Jumping;
        bool isOnGround = Controller.State != PlayerState.Falling
                       && Controller.State != PlayerState.Jumping;

        if (wasInAir && isOnGround)
        {
            TriggerLandingShake();
            _peakFallVelocity = 0f;
        }
    }

    private void TriggerLandingShake()
    {
        if (impulseSource == null) return;

        float fallSpeed = Mathf.Abs(_peakFallVelocity);
        float force = fallSpeed >= hardLandingVelocityThreshold
            ? hardLandingForce
            : softLandingForce;

        impulseSource.GenerateImpulse(force);
    }
}