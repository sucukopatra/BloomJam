using UnityEngine;
using YigitcanCaliskan;
using YigitcanCaliskan.ServiceLocator;

public class FPSInputHandler : MonoBehaviour
{
   public Vector2 MoveInput    { get; private set; }
   public Vector2 LookInput    { get; private set; }
   public bool    IsGamepadActive { get; private set; }
   public bool    JumpPressed  { get; private set; }
   public bool    IsSprinting  { get; private set; }
   public bool    IsCrouching  { get; private set; }

   private void Update()
   {
      var input  = ServiceLocator.Get<IInputService>();
      MoveInput      = input.Move;
      LookInput      = input.Look;
      JumpPressed    = input.JumpPressed;
      IsSprinting    = input.SprintHeld;
      IsCrouching    = input.CrouchHeld;
      IsGamepadActive = input.IsGamepadActive;
   }

   public void ConsumeJump()      => JumpPressed  = false;
   public void ConsumeSprint()    => IsSprinting  = false;
   public void ConsumeCrouching() => IsCrouching  = false;
}