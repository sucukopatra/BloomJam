using UnityEngine;

namespace BloomJam.Weapons
{
    public sealed class WeaponRuntimeContext
    {
        public readonly WeaponController Controller;
        public readonly WeaponData Data;
        public readonly Transform FireOrigin;
        public readonly Transform AimSource;

        public int CurrentAmmo;

        public WeaponRuntimeContext(WeaponController controller, WeaponData data, Transform fireOrigin, Transform aimSource)
        {
            Controller = controller;
            Data = data;
            FireOrigin = fireOrigin;
            AimSource = aimSource;
            CurrentAmmo = data != null ? data.MagazineSize : 0;
        }
    }
}
