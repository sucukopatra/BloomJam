namespace BloomJam.Weapons
{
    /// <summary>
    /// Slot 2 — Green Automatic. Fires continuously while attack is held, gated by fire rate.
    /// </summary>
    public sealed class AutomaticBehaviour : WeaponBehaviourBase
    {
        private bool _triggerHeld;

        public override void OnAttackPressed()  => _triggerHeld = true;
        public override void OnAttackReleased() => _triggerHeld = false;

        public override void OnHolster()
        {
            base.OnHolster();
            _triggerHeld = false;
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
            if (_triggerHeld)
                FireShot();
        }
    }
}
