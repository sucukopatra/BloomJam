using UnityEngine;
using BloomJam.Combat;

namespace BloomJam.Enemies
{
    public class EnemyHitbox : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyMainScript owner;
        [SerializeField] private bool isHeadshot;

        private void Reset()
        {
            owner = GetComponentInParent<EnemyMainScript>();
        }

        public void TakeDamage(in HitInfo hitInfo)
        {
            if (owner == null) return;
            var modified = new HitInfo(
                hitInfo.SourceWeapon,
                hitInfo.Collider,
                hitInfo.Distance,
                hitInfo.BaseDamage,
                isHeadshot
            );
            owner.TakeDamage(in modified);
        }
    }
}