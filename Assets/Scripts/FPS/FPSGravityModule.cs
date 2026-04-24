using UnityEngine;

public class FPSGravityModule : FPSModule
{
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float fallMultiplier    = 1.5f;

    public override int ExecutionOrder => 4; // Jump'tan sonra, Camera'dan önce

    private float _gravity;

    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        _gravity = Physics.gravity.y;
    }

    public override void Tick()
    {
        // Yerdeyken aşağı çekmek gereksiz (CharacterController zaten tutuyor).
        if (Controller.IsGrounded && Controller.Velocity.y < 0f)
            return;

        // Düşerken daha güçlü gravity → daha doğal hissettiren arc
        float multiplier = Controller.Velocity.y < 0f
            ? gravityMultiplier * fallMultiplier
            : gravityMultiplier;

        Controller.Velocity.y += _gravity * multiplier * Time.deltaTime;
        Controller.Velocity.y  = Mathf.Max(Controller.Velocity.y, -30f); // terminal velocity
    }
}