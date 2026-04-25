using System;
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

        // Weapon-related
        bool AttackHeld { get; }
        event Action OnAttack;
        event Action OnReload;
        event Action OnSlot1;
        event Action OnSlot2;
        event Action OnSlot3;
    }
}

