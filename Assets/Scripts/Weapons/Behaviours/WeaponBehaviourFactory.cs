namespace BloomJam.Weapons
{
    public static class WeaponBehaviourFactory
    {
        public static IWeaponBehaviour Create(WeaponBehaviourKind kind) => kind switch
        {
            WeaponBehaviourKind.Pistol    => new PistolBehaviour(),
            WeaponBehaviourKind.Automatic => new AutomaticBehaviour(),
            WeaponBehaviourKind.Shotgun   => new ShotgunBehaviour(),
            _ => new PistolBehaviour(),
        };
    }
}
