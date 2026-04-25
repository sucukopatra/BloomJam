using UnityEngine;
using BloomJam.Combat;
using YigitcanCaliskan.EventBus;

namespace BloomJam.Weapons
{
    /// <summary>
    /// Subscribes to WeaponPelletFiredEvent, raycasts into the world, and delivers a HitInfo
    /// to the first IDamageable on the hit collider's GameObject or any parent.
    ///
    /// This is the entire seam between the weapon system and the enemy/damage system.
    /// Enemy code only needs to implement IDamageable.
    /// </summary>
    public sealed class WeaponHitscan : MonoBehaviour
    {
        [SerializeField, Tooltip("Layers the raycast can hit. Set to whatever layer your enemies + world geometry are on.")]
        private LayerMask _hitMask = ~0;

        [SerializeField, Tooltip("Trigger colliders are included in the raycast when this is on. Off = collide only with solid colliders.")]
        private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;

        [SerializeField, Tooltip("Draw debug rays in the Scene/Game view (with Gizmos on).")]
        private bool _drawDebug = false;

        private void OnEnable()  => EventBus.Subscribe<WeaponPelletFiredEvent>(OnFired);
        private void OnDisable() => EventBus.Unsubscribe<WeaponPelletFiredEvent>(OnFired);

        private void OnFired(WeaponPelletFiredEvent e)
        {
            if (e.Weapon == null) return;

            float range = Mathf.Max(0f, e.Weapon.Range);
            if (range <= 0f) return;

            if (Physics.Raycast(e.Origin, e.Direction, out RaycastHit hit, range, _hitMask, _triggerInteraction))
            {
                if (_drawDebug) Debug.DrawLine(e.Origin, hit.point, Color.yellow, 0.15f);

                IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
                if (target == null) return;

                var info = new HitInfo(
                    sourceWeapon: e.Weapon,
                    collider:     hit.collider,
                    distance:     hit.distance,
                    baseDamage:   e.Weapon.Damage
                );

                target.TakeDamage(in info);
            }
            else if (_drawDebug)
            {
                Debug.DrawRay(e.Origin, e.Direction * range, Color.red, 0.15f);
            }
        }
    }
}
