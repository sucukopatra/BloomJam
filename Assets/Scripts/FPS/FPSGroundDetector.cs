using UnityEngine;

public class FPSGroundDetector : FPSModule
{
    [Header("Zemin Tespiti")]
    [SerializeField] private LayerMask groundLayer = 0;
    [SerializeField] private float checkRadius = 0.28f;
    [SerializeField] private float groundedOffset  = -0.14f;
    
    public override int ExecutionOrder => 0; // İlk çalışır: IsGrounded diğer modüllere hazır olsun

    private bool _wasGrounded;
    private Vector3 spherePosition;

    public override void Tick()
    {
        _wasGrounded = Controller.IsGrounded;
        
        spherePosition = new Vector3(
            Controller.transform.position.x,
            Controller.transform.position.y + groundedOffset,
            Controller.transform.position.z
        );
        
        Controller.IsGrounded = Physics.CheckSphere(spherePosition, checkRadius, groundLayer, QueryTriggerInteraction.Ignore);
        
        if (Controller.IsGrounded && !_wasGrounded)
        {
            if (Controller.Velocity.y < 0f)
                Controller.Velocity.y = -2f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Controller.IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(spherePosition, checkRadius);
    }
    
    public override void Initialize(FPSController controller)
    {
        base.Initialize(controller);
        
    }
}
