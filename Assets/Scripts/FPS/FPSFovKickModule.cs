using UnityEngine;
using Unity.Cinemachine;

public class FPSFovKickModule : FPSModule
{
    public override int ExecutionOrder => 8;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float baseFov   = 60f;
    [SerializeField] private float sprintFov = 70f;
    [SerializeField] private float kickSpeed = 8f;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        if (virtualCamera != null)
            virtualCamera.m_Lens.FieldOfView = baseFov;
    }

    public override void LateTick()
    {
        if (virtualCamera == null) return;

        float targetFov = Controller.IsSprinting ? sprintFov : baseFov;

        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.m_Lens.FieldOfView,
            targetFov,
            kickSpeed * Time.deltaTime
        );
    }
}