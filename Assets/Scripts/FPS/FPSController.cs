using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(FPSInputHandler))]
public class FPSController : MonoBehaviour
{
    [HideInInspector] public CharacterController CharController;
    [HideInInspector] public FPSInputHandler Input;
    [HideInInspector] public Vector3 Velocity;
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool IsCrouching;
    public PlayerState State;
    public PlayerState PreviousState;

    // Tek noktadan atanır; hem CameraModule hem CrouchingModule kullanır.
    public Transform CameraRoot;

    private FPSModule[] _modules;

    private void Awake()
    {
        CharController = GetComponent<CharacterController>();
        Input = GetComponent<FPSInputHandler>();

        // ExecutionOrder'a göre sırala: Ground(0)→Crouch(1)→Move(2)→Jump(3)→Gravity(4)→State(5)→Camera(6)
        _modules = GetComponents<FPSModule>()
            .OrderBy(m => m.ExecutionOrder)
            .ToArray();

        foreach (var m in _modules)
            m.Initialize(this);

        InputManager.Instance.SwitchToGameplay();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        foreach (var m in _modules)
            if (m.enabled) m.Tick();

        CharController.Move(Velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        foreach (var m in _modules)
            if (m.enabled) m.LateTick();
    }
}