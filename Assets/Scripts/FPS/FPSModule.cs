using UnityEngine;

public abstract class FPSModule : MonoBehaviour
{
    // Düşük sayı = önce çalışır. Ground(0) → Crouch(1) → Move(2) → Jump(3) → Gravity(4) → Camera(5)
    public virtual int ExecutionOrder => 0;

    protected FPSController Controller { get; private set; }

    public virtual void Initialize(FPSController controller)
    {
        Controller = controller;
    }

    public virtual void Tick() { }

    public virtual void LateTick() { }
}
