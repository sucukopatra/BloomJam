using UnityEngine;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    public abstract class WeaponBehaviourBase : IWeaponBehaviour
    {
        protected WeaponRuntimeContext Ctx;
        protected float CooldownRemaining;

        public virtual void Initialize(WeaponRuntimeContext ctx)
        {
            Ctx = ctx;
            CooldownRemaining = 0f;
        }

        public virtual void OnEquip()
        {
            CooldownRemaining = 0f;
        }

        public virtual void OnHolster() { }

        public virtual void Tick(float dt)
        {
            if (CooldownRemaining > 0f)
                CooldownRemaining -= dt;
        }

        public virtual void OnAttackPressed() { }
        public virtual void OnAttackReleased() { }

        protected bool CanFire()
        {
            if (Ctx?.Data == null) return false;
            if (Ctx.Controller == null || Ctx.Controller.IsBusy) return false;
            if (CooldownRemaining > 0f) return false;
            return true;
        }

        protected bool TryConsumeAmmo()
        {
            if (Ctx.CurrentAmmo <= 0)
            {
                EventBus.Publish(new WeaponDryFireEvent(Ctx.Data));
                // Reset cooldown so dry-fire isn't spammable faster than the weapon's fire rate.
                CooldownRemaining = 1f / Mathf.Max(0.01f, Ctx.Data.FireRate);
                if (Ctx.Data.AutoReloadOnEmpty)
                    Ctx.Controller.RequestReload();
                return false;
            }

            Ctx.CurrentAmmo--;
            EventBus.Publish(new AmmoChangedEvent(Ctx.Data, Ctx.CurrentAmmo, Ctx.Data.MagazineSize));
            return true;
        }

        /// <summary>
        /// Common shot pipeline: cooldown gate, ammo decrement, pellet loop, event publish.
        /// Returns true if a shot actually fired.
        /// </summary>
        protected bool FireShot()
        {
            if (!CanFire()) return false;
            if (!TryConsumeAmmo()) return false;

            CooldownRemaining = 1f / Mathf.Max(0.01f, Ctx.Data.FireRate);

            // Origin priority: muzzle > aim source > abort. Vector3.zero would shoot
            // from world origin, which silently breaks hit detection.
            Vector3 origin;
            if (Ctx.FireOrigin != null)      origin = Ctx.FireOrigin.position;
            else if (Ctx.AimSource != null)  origin = Ctx.AimSource.position;
            else
            {
                Debug.LogError($"[{nameof(WeaponBehaviourBase)}] No FireOrigin or AimSource on {Ctx.Data?.DisplayName}; aborting shot.");
                return false;
            }

            Vector3 baseDir = Ctx.AimSource != null ? Ctx.AimSource.forward
                            : Ctx.FireOrigin != null ? Ctx.FireOrigin.forward
                            : Vector3.forward;

            int pellets = Mathf.Max(1, Ctx.Data.PelletsPerShot);

            // Per-shot event drives view layer (VFX/audio/anim/screen flash) — fires once.
            EventBus.Publish(new WeaponFiredEvent(Ctx.Data, origin, baseDir, Ctx.CurrentAmmo));

            // Per-pellet event drives hit detection — fires N times with independent spread.
            for (int i = 0; i < pellets; i++)
            {
                Vector3 dir = ApplySpread(baseDir, Ctx.Data.SpreadDegrees);
                EventBus.Publish(new WeaponPelletFiredEvent(Ctx.Data, origin, dir));
            }

            if (Ctx.CurrentAmmo == 0 && Ctx.Data.AutoReloadOnEmpty)
                Ctx.Controller.RequestReload();

            return true;
        }

        protected static Vector3 ApplySpread(Vector3 forward, float coneHalfAngleDeg)
        {
            if (coneHalfAngleDeg <= 0f) return forward;

            // Random cone deflection: uniform in disc, then rotate forward by that.
            float yaw   = Random.Range(-coneHalfAngleDeg, coneHalfAngleDeg);
            float pitch = Random.Range(-coneHalfAngleDeg, coneHalfAngleDeg);
            Quaternion rot = Quaternion.LookRotation(forward) * Quaternion.Euler(pitch, yaw, 0f);
            return rot * Vector3.forward;
        }
    }
}
