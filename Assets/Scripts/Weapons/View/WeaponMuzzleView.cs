using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Spawns the per-weapon muzzle flash + particle prefabs at the muzzle anchor on each shot.
    /// Tints both to the weapon's color. Both prefabs are TODO art on WeaponData.
    /// </summary>
    public sealed class WeaponMuzzleView : MonoBehaviour
    {
        [SerializeField, Tooltip("Anchor where flash + particles spawn. Usually the fingertip / barrel tip.")]
        private Transform _muzzleAnchor;

        [SerializeField, Tooltip("Lifetime of the flash GameObject before it's destroyed.")]
        [Min(0.01f)] private float _flashLifetime = 0.05f;

        private void OnEnable()  => EventBus.Subscribe<WeaponFiredEvent>(OnFired);
        private void OnDisable() => EventBus.Unsubscribe<WeaponFiredEvent>(OnFired);

        private void OnFired(WeaponFiredEvent e)
        {
            if (_muzzleAnchor == null) return;

            if (e.Weapon.MuzzleFlashPrefab != null)
            {
                var flash = Instantiate(e.Weapon.MuzzleFlashPrefab, _muzzleAnchor.position, _muzzleAnchor.rotation, _muzzleAnchor);
                Tint(flash, e.Weapon.TintColor);
                Destroy(flash, _flashLifetime);
            }

            if (e.Weapon.MuzzleParticlePrefab != null)
            {
                var fx = Instantiate(e.Weapon.MuzzleParticlePrefab, _muzzleAnchor.position, _muzzleAnchor.rotation, _muzzleAnchor);
                Tint(fx, e.Weapon.TintColor);
                // Particles auto-destroy via their own Stop Action; we don't manage lifetime here.
            }
        }

        private static void Tint(GameObject go, Color c)
        {
            var ps = go.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = c;
            }
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = c;
        }
    }
}
