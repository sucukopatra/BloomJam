using UnityEngine;
using YigitcanCaliskan;
public class FPSMovementModule : FPSModule
{
   public override int ExecutionOrder => 2; // Crouch(1) sonrası: IsCrouching okuyabilsin

   [SerializeField] private float walkSpeed = 5f;
   [SerializeField] private float sprintSpeed = 9f;
   [SerializeField] private float crouchSpeedMultiplier = 0.5f;

   [SerializeField] private float acceleration = 10f;
   [SerializeField] private float deceleration = 10f;
   
   private Vector3 _currentHorizontalVelocity;

   public override void Initialize(FPSController controller)
   {
       base.Initialize(controller);
   }
   
   public override void Tick()
   {
      Vector2 input = Controller.Input.MoveInput;
      
      Vector3 moveDirection = Controller.transform.right   * input.x
                              + Controller.transform.forward * input.y;
      
      bool wantsToSprint = Controller.Input.IsSprinting && input.y > 0.1f;
      bool canSprint = wantsToSprint && !Controller.IsCrouching;
      float crouchMultiplier = Controller.IsCrouching ? crouchSpeedMultiplier : 1f;
      float targetSpeed  = (canSprint ? sprintSpeed : walkSpeed) * crouchMultiplier;
      Controller.IsSprinting = canSprint;
      
      Vector3 targetVelocity = moveDirection.normalized * (input.magnitude > 0.1f ? targetSpeed : 0f);
      
      float smoothFactor = input.magnitude > 0.1f ? acceleration : deceleration;
      _currentHorizontalVelocity = Vector3.Lerp(
          _currentHorizontalVelocity,
          targetVelocity,
          smoothFactor * Time.deltaTime
      );

      Controller.Velocity.x = _currentHorizontalVelocity.x;
      Controller.Velocity.z = _currentHorizontalVelocity.z;
   }
}
