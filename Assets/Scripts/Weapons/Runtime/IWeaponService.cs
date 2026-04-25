namespace BloomJam.Weapons
{
    /// <summary>
    /// Read-only view of weapon state for HUD / downstream systems.
    /// State changes are pushed through EventBus; this interface is for snapshot reads only.
    /// </summary>
    public interface IWeaponService
    {
        WeaponData CurrentWeapon { get; }
        int CurrentSlot { get; }
        int CurrentAmmo { get; }
        int MaxAmmo { get; }
        bool IsBusy { get; }
    }
}
