namespace BloomJam.Weapons
{
    /// <summary>
    /// Slot 3 — Blue Shotgun. Semi-auto: one trigger pull fires N pellets in a cone.
    /// Pellet count and spread come from WeaponData.
    /// </summary>
    public sealed class ShotgunBehaviour : WeaponBehaviourBase
    {
        public override void OnAttackPressed()
        {
            FireShot();
        }
    }
}
