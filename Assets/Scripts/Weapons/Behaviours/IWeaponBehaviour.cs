namespace BloomJam.Weapons
{
    public interface IWeaponBehaviour
    {
        void Initialize(WeaponRuntimeContext ctx);
        void OnEquip();
        void OnHolster();
        void Tick(float dt);
        void OnAttackPressed();
        void OnAttackReleased();
    }
}
