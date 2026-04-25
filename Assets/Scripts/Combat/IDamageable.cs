namespace BloomJam.Combat
{
    /// <summary>
    /// The single contract the enemy/damage system implements.
    ///
    /// The weapon system raycasts on each shot, finds the first IDamageable on the hit
    /// collider's GameObject or any parent, and calls TakeHit exactly once per pellet.
    ///
    /// Implementer responsibilities:
    ///   - Decide whether the hit applies (e.g. filter by hit.SourceWeapon.BehaviourKind).
    ///   - Compute the actual damage (multiply by body-part factor, ignore, etc.).
    ///   - Apply the damage and trigger reactions / animations / VFX on its own side.
    ///   - Optionally publish DamageDealtEvent / EnemyKilledEvent so the weapon side can
    ///     show a hit-marker, play a confirm sound, etc.
    ///
    /// The weapon system never reads a return value; it never asks how much damage was
    /// applied. All feedback flows back through events.
    /// </summary>
    public interface IDamageable
    {
        void TakeHit(in HitInfo hit);
    }
}
