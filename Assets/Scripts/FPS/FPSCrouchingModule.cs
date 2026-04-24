using UnityEngine;

public class FPSCrouchingModule : FPSModule
{
    [SerializeField] private float standHeight     = 1.8f;
    [SerializeField] private float crouchHeight    = 0.9f;
    [SerializeField] private float transitionSpeed = 8f;

    [Header("Kamera")]
    [SerializeField] private float standEyeHeight  = 1.6f;
    [SerializeField] private float crouchEyeHeight = 0.6f;

    public override int ExecutionOrder => 1; // Ground(0) → Crouch(1) → Move(2)

    private float _targetHeight;
    private float _targetEyeHeight;
    private float _lerpedEyeHeight;

    // HeadBobModule bu değeri okur; CameraRoot.localPosition'dan okumak bob offset'ini kirletir
    public float LerpedEyeHeight => _lerpedEyeHeight;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        Controller.CharController.height = standHeight;
        _targetHeight    = standHeight;
        _targetEyeHeight = standEyeHeight;
        _lerpedEyeHeight = standEyeHeight;
    }

    public override void Tick()
    {
        bool wants = Controller.Input.IsCrouching;
        if (!wants && CeilingBlocked()) wants = true;

        Controller.IsCrouching = wants;
        _targetHeight    = wants ? crouchHeight    : standHeight;
        _targetEyeHeight = wants ? crouchEyeHeight : standEyeHeight;

        var cc = Controller.CharController;
        cc.height = Mathf.Lerp(cc.height, _targetHeight, transitionSpeed * Time.deltaTime);
        cc.center = new Vector3(0, cc.height / 2f, 0);

        // Lerp kendi dahili field'ından beslendiği için HeadBob'un eklediği offset sızmaz
        _lerpedEyeHeight = Mathf.Lerp(_lerpedEyeHeight, _targetEyeHeight, transitionSpeed * Time.deltaTime);

        if (Controller.CameraRoot != null)
        {
            Vector3 pos = Controller.CameraRoot.localPosition;
            pos.y = _lerpedEyeHeight;
            Controller.CameraRoot.localPosition = pos;
        }
    }

    private bool CeilingBlocked()
    {
        var cc = Controller.CharController;
        Vector3 castOrigin = Controller.transform.position
                             + Vector3.up * (cc.height - cc.radius);

        return Physics.SphereCast(
            castOrigin,
            cc.radius,
            Vector3.up,
            out _,
            standHeight - crouchHeight + 0.15f
        );
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = CeilingBlocked() ? Color.green : Color.red;
        var cc = Controller.CharController;
        Vector3 castOrigin = Controller.transform.position
                             + Vector3.up * (cc.height - cc.radius);
        Gizmos.DrawWireSphere(castOrigin, cc.radius);
    }
}