using UnityEngine;
using YigitcanCaliskan.EventBus;
using BloomJam.Combat;
using BloomJam.Weapons;

namespace BloomJam.Enemies
{
    public class EnemyMainScript : MonoBehaviour, IDamageable
    {
        [Header("Identity")]
        [SerializeField] private EnemyType type;
        [SerializeField] private WeaponBehaviourKind weakTo;
        [SerializeField] private int pairingId;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxshotgundistance = 100f;

        [Header("Refs")]
        [SerializeField] private EnemyAnimator enemyAnimator;
        [SerializeField] private EnemyAI enemyAI;

        public float CurrentHealth { get; private set; }
        public EnemyType Type => type;

        private bool _isDead;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(in HitInfo hitinfo)
        {
            if (_isDead) return;
            if (hitinfo.SourceWeapon.BehaviourKind != weakTo) return;

            var newDamage = 0f;
            if (maxshotgundistance > hitinfo.Distance)
            {
                newDamage = hitinfo.SourceWeapon.BehaviourKind == WeaponBehaviourKind.Shotgun
                    ? hitinfo.BaseDamage * (1 - (hitinfo.Distance / maxshotgundistance))
                    : hitinfo.BaseDamage;
            }

            CurrentHealth -= (int)newDamage;

            if (CurrentHealth <= 0f)
            {
                Die(hitinfo.IsHeadshot);
            }
            else
            {
                enemyAnimator.PlayHurt();
            }
        }

        private void Die(bool isHeadshot)
        {
            _isDead = true;
            enemyAI.OnDeath();

            if (isHeadshot) enemyAnimator.PlayDieHead();
            else enemyAnimator.PlayDieNormal();

            EventBus.Publish(new EnemyDiedEvent
            {
                Type = type,
                PairingId = pairingId,
                Position = transform.position
            });

            // Animasyonun bitmesi için biraz bekle, sonra destroy
            Destroy(gameObject, 1.5f);
        }

        // Test için bıraktığın jump-to-die kısmını sildim,
        // production'a geçmeden temiz olsun
    }
}