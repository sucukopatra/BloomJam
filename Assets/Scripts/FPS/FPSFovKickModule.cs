using UnityEngine;
using Unity.Cinemachine;

public class FPSFovKickModule : FPSModule
{
    public override int ExecutionOrder => 8;

    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float baseFov   = 60f;
    [SerializeField] private float sprintFov = 70f;
    [SerializeField] private float kickSpeed = 8f;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        if (cinemachineCamera != null)
            cinemachineCamera.Lens.FieldOfView = baseFov;
    }

    public override void LateTick()
    {
        if (cinemachineCamera == null) return;

        float targetFov = Controller.IsSprinting ? sprintFov : baseFov;

        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(
            cinemachineCamera.Lens.FieldOfView,
            targetFov,
            kickSpeed * Time.deltaTime
        );
    }
}
