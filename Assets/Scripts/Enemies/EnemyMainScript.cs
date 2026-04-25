using UnityEngine;
using YigitcanCaliskan.EventBus;
using BloomJam.Combat;

namespace BloomJam.Enemies
{
    [RequireComponent(typeof(Collider))]
    public class EnemyMainScript : MonoBehaviour, IDamageable
    {
        [Header("Identity")]
        [SerializeField] private EnemyType type;
        [SerializeField] private WeaponType weakTo;
        [SerializeField] private int pairingId;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;

        public float CurrentHealth { get; private set; }
        public EnemyType Type => type;

        private bool _isDead;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(in HitInfo hitinfo)
        {
            // Multiple pellets in one frame can drive HP below zero before Destroy
            // takes effect, so guard against double-Die.
            if (_isDead) return;

            CurrentHealth -= hitinfo.BaseDamage;

            if (CurrentHealth <= 0f)
            {
                _isDead = true;
                Die();
            }
        }

        private void Die()
        {
            EventBus.Publish(new EnemyDiedEvent
            {
                Type = type,
                PairingId = pairingId,
                Position = transform.position
            });

            Destroy(gameObject);
        }
    }
}
