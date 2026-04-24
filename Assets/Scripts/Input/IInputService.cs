using UnityEngine;

namespace YigitcanCaliskan
{
    public interface IInputService
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool JumpPressed { get; }
        bool JumpHeld { get; }
        bool JumpReleased { get; }
        bool SprintHeld { get; }
        bool CrouchHeld { get; }
        bool IsGamepadActive { get; }
    }
}

