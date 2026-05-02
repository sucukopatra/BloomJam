using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCameraModule : FPSModule
{
    [Header("Hassasiyet (Mouse)")]
    [SerializeField] private float sensitivityX = 0.15f;
    [SerializeField] private float sensitivityY = 0.15f;

    [Header("Hassasiyet (Gamepad - derece/saniye)")]
    [SerializeField] private float gamepadSensitivityX = 180f;
    [SerializeField] private float gamepadSensitivityY = 180f;

    [Header("Dikey Bakış Limiti")]
    [SerializeField] private float topClamp    = -80f;
    [SerializeField] private float bottomClamp =  80f;

    [Header("Yumuşatma")]
    [Tooltip("0 = yumuşatma yok (anlık), 1'e yakın = çok yumuşak (gecikmeli).")]
    [Range(0f, 0.99f)]
    [SerializeField] private float smoothing = 0f;

    public override int ExecutionOrder => 6; 
    
    private float _yaw;
    private float _pitch;
    private float _targetYaw;
    private float _targetPitch;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);

        _yaw   = Controller.transform.eulerAngles.y;
        _pitch = Controller.CameraRoot != null ? Controller.CameraRoot.localEulerAngles.x : 0f;

        _targetYaw   = _yaw;
        _targetPitch = _pitch;
    }
    // Kamera LateUpdate'te güncellenir: fizik titremesini önler
    public override void LateTick()
    {
        Vector2 look = Controller.Input.LookInput;

        // Gamepad sabit eksen (-1..1) verir → Time.deltaTime ile ölçeklenir.
        // Mouse kare başına delta verir → Time.deltaTime gerekmez.
        float sx, sy;
        if (Controller.Input.IsGamepadActive)
        {
            sx = gamepadSensitivityX * Time.deltaTime;
            sy = gamepadSensitivityY * Time.deltaTime;
        }
        else
        {
            sx = sensitivityX;
            sy = sensitivityY;
        }

        _targetYaw   += look.x * sx;
        _targetPitch -= look.y * sy;
        _targetPitch  = Mathf.Clamp(_targetPitch, topClamp, bottomClamp);

        if (smoothing > 0f)
        {
            float t = 1f - smoothing;
            _yaw   = Mathf.Lerp(_yaw,   _targetYaw,   t);
            _pitch = Mathf.Lerp(_pitch, _targetPitch, t);
        }
        else
        {
            _yaw   = _targetYaw;
            _pitch = _targetPitch;
        }

        Controller.transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

        if (Controller.CameraRoot != null)
            Controller.CameraRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

}