using UnityEngine;
using BloomJam.Weapons;

namespace BloomJam.Combat
{
    /// <summary>
    /// All the information about a single hit, delivered to <see cref="IDamageable"/>.
    /// Built by the weapon system; consumed by the enemy/damage system.
    ///
    /// The struct carries everything the receiver needs to decide what to do:
    ///   - <see cref="SourceWeapon"/> for color/kind filtering ("only Red Pistol damages me")
    ///   - <see cref="Collider"/> so the receiver can route to a body part it owns
    ///   - <see cref="BaseDamage"/> so the receiver can multiply (headshot, weakpoint) before applying
    ///   - geometry (Point/Normal/Direction) for VFX, knockback, ragdoll force, etc.
    /// </summary>
    public readonly struct HitInfo
    {
        /// <summary>The WeaponData that fired the shot. Use BehaviourKind / TintColor to filter vulnerability.</summary>
        public readonly WeaponData SourceWeapon;

        /// <summary>The exact collider that the raycast hit. Use this to identify a body part.</summary>
        public readonly Collider Collider;

        /// <summary>World-space hit point.</summary>
        public readonly Vector3 Point;

        /// <summary>Surface normal at the hit point.</summary>
        public readonly Vector3 Normal;

        /// <summary>Normalized direction of the bullet (from origin to point).</summary>
        public readonly Vector3 Direction;

        /// <summary>Distance from fire origin to hit point.</summary>
        public readonly float Distance;

        /// <summary>WeaponData.Damage as fired. The receiver may multiply / zero this out before applying.</summary>
        public readonly float BaseDamage;

        public HitInfo(WeaponData sourceWeapon, Collider collider, Vector3 point, Vector3 normal, Vector3 direction, float distance, float baseDamage)
        {
            SourceWeapon = sourceWeapon;
            Collider = collider;
            Point = point;
            Normal = normal;
            Direction = direction;
            Distance = distance;
            BaseDamage = baseDamage;
        }
    }
}
