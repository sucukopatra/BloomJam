namespace BloomJam.Weapons
{
    /// <summary>
    /// Slot 1 — Red Pistol. Semi-automatic: one shot per click, gated by fire rate.
    /// </summary>
    public sealed class PistolBehaviour : WeaponBehaviourBase
    {
        public override void OnAttackPressed()
        {
            FireShot();
        }
    }
}
