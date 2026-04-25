using UnityEngine;
using BloomJam.Weapons;

namespace BloomJam.Combat
{
    public readonly struct HitInfo
    {
        public readonly WeaponData SourceWeapon;
        public readonly Collider Collider;
        public readonly float Distance;
        public readonly float BaseDamage;

        public HitInfo(WeaponData sourceWeapon, Collider collider, float distance, float baseDamage)
        {
            SourceWeapon = sourceWeapon;
            Collider = collider;
            Distance = distance;
            BaseDamage = baseDamage;
        }
    }
}
