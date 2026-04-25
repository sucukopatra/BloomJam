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

        public HitInfo(WeaponData sourceWeapon, Collider collider, Vector3 point, Vector3 normal, Vector3 direction, float distance, float baseDamage)
        {
            SourceWeapon = sourceWeapon;
            Collider = collider;
            Distance = distance;
            BaseDamage = baseDamage;
        }
    }
}
