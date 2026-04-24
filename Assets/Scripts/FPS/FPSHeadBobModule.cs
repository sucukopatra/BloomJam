using UnityEngine;

public class FPSHeadBobModule : FPSModule
{
    public override int ExecutionOrder => 7;

    [Header("Walk")]
    [SerializeField] private float walkFrequency  = 1.8f;
    [SerializeField] private float walkAmplitudeY = 0.05f;
    [SerializeField] private float walkAmplitudeX = 0.025f;

    [Header("Sprint")]
    [SerializeField] private float sprintFrequency  = 2.5f;
    [SerializeField] private float sprintAmplitudeY = 0.1f;
    [SerializeField] private float sprintAmplitudeX = 0.05f;

    [Header("Crouch")]
    [SerializeField] private float crouchFrequency  = 1.2f;
    [SerializeField] private float crouchAmplitudeY = 0.025f;
    [SerializeField] private float crouchAmplitudeX = 0.015f;

    [Header("Smoothing")]
    [SerializeField] private float amplitudeSmoothing = 8f;

    private FPSCrouchingModule _crouching;
    private float _fallbackEyeHeight;
    private float _bobTimer;
    private float _currentAmpY;
    private float _currentAmpX;
    private float _currentFreq;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        _crouching = controller.GetComponent<FPSCrouchingModule>();
        _fallbackEyeHeight = Controller.CameraRoot != null ? Controller.CameraRoot.localPosition.y : 0f;
    }

    public override void LateTick()
    {
        if (Controller.CameraRoot == null) return;

        bool canBob = Controller.IsGrounded
                   && (Controller.State == PlayerState.Walking
                    || Controller.State == PlayerState.Sprinting
                    || Controller.State == PlayerState.Crouching);

        float targetAmpY, targetAmpX, targetFreq;

        if (canBob)
        {
            switch (Controller.State)
            {
                case PlayerState.Sprinting:
                    targetAmpY = sprintAmplitudeY; targetAmpX = sprintAmplitudeX; targetFreq = sprintFrequency;
                    break;
                case PlayerState.Crouching:
                    targetAmpY = crouchAmplitudeY; targetAmpX = crouchAmplitudeX; targetFreq = crouchFrequency;
                    break;
                default:
                    targetAmpY = walkAmplitudeY; targetAmpX = walkAmplitudeX; targetFreq = walkFrequency;
                    break;
            }
        }
        else
        {
            targetAmpY = 0f; targetAmpX = 0f; targetFreq = _currentFreq;
        }

        float t = amplitudeSmoothing * Time.deltaTime;
        _currentAmpY = Mathf.Lerp(_currentAmpY, targetAmpY, t);
        _currentAmpX = Mathf.Lerp(_currentAmpX, targetAmpX, t);
        _currentFreq = Mathf.Lerp(_currentFreq, targetFreq, t);

        _bobTimer += Time.deltaTime * _currentFreq;

        float bobY = Mathf.Sin(_bobTimer * Mathf.PI * 2f) * _currentAmpY;
        float bobX = Mathf.Cos(_bobTimer * Mathf.PI)      * _currentAmpX;

        // += yerine doğrudan SET: x birikmez, y CrouchingModule'ün temiz değerinden alınır
        float baseY = _crouching != null ? _crouching.LerpedEyeHeight : _fallbackEyeHeight;
        Controller.CameraRoot.localPosition = new Vector3(bobX, baseY + bobY, 0f);
    }
}